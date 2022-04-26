using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEditor;
using System;
using TMPro;
using UnityEngine.Animations.Rigging;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Match_UI_Manager : MonoBehaviour
{
    public GameObject charaButton;

    private bool allieSelected;

    //Data
    private PlayerSpecs _playerCaptain;
    private MateSpecs _playerAlly;
    private PlayerSpecs _AICaptain;
    private MateSpecs _AIAlly;

    [HideInInspector] public int gameTime;
    [HideInInspector] public float goalToWin;
    [HideInInspector] public int AIDifficulty;
    //

    //Chara buttons
    [SerializeField]
    private GameObject mainCharacterSelection;
    private GameObject FS_mainCharacter;

    [SerializeField]
    private GameObject allieSelection;
    private GameObject FS_allie;

    [SerializeField]
    private GameObject matchSettings;
    [SerializeField]
    private GameObject FS_matchSettings;

    [SerializeField]
    private EventSystem ES;

    public List<GameObject> matchParams = new List<GameObject>();

    public List<PlayerSpecs> maincharaSpecs = new List<PlayerSpecs>();
    public List<MateSpecs> mateSpecs = new List<MateSpecs>();
    private List<PlayerSpecs> _onlyMateSpecs = new List<PlayerSpecs>();
    //public List<PlayerSpecs> alliecharaSpecs = new List<PlayerSpecs>();

    List<Button> charaButtons;
    List<Button> allieButtons;

    private GameObject actualSelectedGameObject;

    //Stats
    public TextMeshProUGUI accuracy;
    public TextMeshProUGUI speed;
    public TextMeshProUGUI stunTime;
    //

    public static Match_UI_Manager _instance;

    private void Awake()
    {
        _instance = this;
    }
    private void Start()
    {

        allieSelected = false;
        FS_mainCharacter=InitCharacters(mainCharacterSelection);

        ES.SetSelectedGameObject(FS_mainCharacter);

        //UpdateStat
        actualSelectedGameObject = FS_mainCharacter;
        UpdateStat(actualSelectedGameObject);

        foreach (var param in matchParams)
        {
            ISliderValue slider = param.GetComponent<ISliderValue>();

            if (slider != null)
                slider.OnValueChange(param.GetComponentInChildren<Slider>().value);
        }

        //Initialize onlyMateSpecs
        foreach(var elem in mateSpecs)
        {
            _onlyMateSpecs.Add( elem.mateSpec);
        }
    }
    private void Update()
    {
        //MatchSetting
        if(matchSettings.activeSelf)
        {
            if((Keyboard.current?.enterKey.wasPressedThisFrame ?? false) || (Gamepad.current?.buttonSouth.wasPressedThisFrame ?? false))
            {
                Slider sliderGo = null;
                if(ES.currentSelectedGameObject.TryGetComponent(out sliderGo))
                    ES.SetSelectedGameObject(sliderGo.navigation.selectOnDown.transform.gameObject);
            }
        }

        //UpdateStatChara
        if(ES.currentSelectedGameObject != actualSelectedGameObject && !matchSettings.activeSelf)
        {
            actualSelectedGameObject = ES.currentSelectedGameObject;
            UpdateStat(actualSelectedGameObject);
        }

        //GoBack
        if ((Keyboard.current?.escapeKey.wasPressedThisFrame ?? false) || (Gamepad.current?.buttonEast.wasPressedThisFrame ?? false))
        {
            if (matchSettings.activeSelf)
            {
                matchSettings.SetActive(false);
                allieSelection.SetActive(true);
                ES.SetSelectedGameObject(FS_allie);
            }
            else if (allieSelection.activeSelf)
            {
                allieSelection.SetActive(false);
                mainCharacterSelection.SetActive(true);
                ES.SetSelectedGameObject(FS_mainCharacter);
            }
            else
                LevelLoader.LoadNextLevel(0);
        }
    }

    private void UpdateStat(GameObject button)
    {
        PlayerSpecs chara;

        if (allieSelection.activeSelf)
            chara = mateSpecs[allieButtons.IndexOf(button.GetComponent<Button>())].mateSpec;
        else
            chara = maincharaSpecs[charaButtons.IndexOf(button.GetComponent<Button>())];

        accuracy.text = "Accuracy: "+ chara.Accuracy;
        speed.text = "Speed :" + chara.Speed;
        stunTime.text = "StunTime: " + chara.StunTime;
    }

    GameObject InitCharacters(GameObject Step)
    {
        GridLayoutGroup CharaSection = Step.GetComponentInChildren<GridLayoutGroup>();
        GameObject FS=null;

        List<Button> Buttons = new List<Button>();


        Quaternion rot = Quaternion.Euler(new Vector3(0, 180, 0));
        Vector3 pos = new Vector3(0, -25, -15);
        Vector3 scale = new Vector3(30, 30, 30);

        List<PlayerSpecs> charaspecs;

        if(allieSelection.activeSelf)
        {
            charaspecs = _onlyMateSpecs;
        }
        else
        {
            charaspecs = maincharaSpecs;
        }
        for (int i = 0; i< charaspecs.Count; i++)
        {
            GameObject newChara = Instantiate(charaButton,CharaSection.transform);
            newChara.name = charaspecs[i].Name;

            GameObject charaPrefab = Instantiate(charaspecs[i].StaticPrefab, newChara.transform);
            charaPrefab.transform.localPosition = pos;
            charaPrefab.transform.localScale = scale;
            charaPrefab.transform.localRotation = rot;

            Button btn = newChara.GetComponent<Button>();

            PlayerSpecs spectopass = charaspecs[i];
            btn.onClick.AddListener(() => OnContinue(spectopass));

            EventTrigger ET = newChara.GetComponent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Deselect;
            entry.callback.AddListener((data) => { OnSelected(); });
            ET.triggers.Add(entry);

            Buttons.Add(btn);

            if(i==0)
            {
                FS = newChara;
            }
        }

        Navigation btnNav = new Navigation();
        for (int i =0; i< Buttons.Count; i++)
        {
            btnNav.mode = Navigation.Mode.Explicit;

            if(i==0)
            {
                btnNav.selectOnRight = Buttons[i+1];
                btnNav.selectOnLeft = Buttons[Buttons.Count-1];
            }
            else if(i== Buttons.Count-1)
            {
                btnNav.selectOnRight = Buttons[0];
                btnNav.selectOnLeft = Buttons[i-1];
            }
            else
            {
                btnNav.selectOnRight = Buttons[i + 1];
                btnNav.selectOnLeft = Buttons[i - 1];
            }

            Buttons[i].navigation = btnNav;
        }

        if(allieSelection.activeSelf)
        {
            allieButtons = Buttons;
        }
        else
        {
            charaButtons = Buttons;
        }

        return FS;
    }

    public void OnSelected()
    {
        AudioManager._instance.PlaySFX(AudioManager.SFXType.ButtonSelected);
    }

    public void OnContinue(PlayerSpecs CharaSpec)
    {
        AudioManager._instance.PlaySFX(AudioManager.SFXType.ButtonClicked);

        if (allieSelection.activeSelf)
        {
            ES.SetSelectedGameObject(FS_matchSettings);
            matchSettings.SetActive(true);

            allieSelection.SetActive(false);

            _playerAlly = mateSpecs[_onlyMateSpecs.IndexOf(CharaSpec)];
        }
        else if(!allieSelected)
        {
            allieSelected = true;

            allieSelection.SetActive(true);

            FS_allie = InitCharacters(allieSelection);

            mainCharacterSelection.SetActive(false);
            ES.SetSelectedGameObject(FS_allie);

            _playerCaptain = CharaSpec;
        }
        else
        {
            mainCharacterSelection.SetActive(false);
            allieSelection.SetActive(true);
            ES.SetSelectedGameObject(FS_allie);
            _playerCaptain = CharaSpec;
        }
    }

    public void OnPlay()
    {
        AudioManager._instance.PlaySFX(AudioManager.SFXType.ButtonClicked);

        GetRandomEnnemies();

        GameManager.AddMatch(_playerCaptain, _playerAlly.mateSpec, _AICaptain, _AIAlly.mateSpec,gameTime,goalToWin,AIDifficulty);
        AudioManager._instance.SetCharaAudio(_playerCaptain, _playerAlly.mateSpec, _AICaptain, _AIAlly.mateSpec);

        LevelLoader.LoadNextLevel(SceneManager.GetActiveScene().buildIndex + 1);
    }

    private void GetRandomEnnemies()
    {
        List<PlayerSpecs> charas = new List<PlayerSpecs>();

        foreach (var chara in maincharaSpecs)
        {
            if (chara != _playerCaptain)
                charas.Add(chara);
        }
        
        _AICaptain = charas[Random.Range(0, charas.Count)];


        charas.Clear();

        foreach (var chara in _onlyMateSpecs)
        {
            if (mateSpecs[_onlyMateSpecs.IndexOf(chara)].mateType != _playerAlly.mateType) 
                charas.Add(chara);
        }

        _AIAlly = mateSpecs[_onlyMateSpecs.IndexOf(charas[Random.Range(0, charas.Count)])];
    }
    
}
