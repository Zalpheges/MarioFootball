using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Match_UI_Manager : MonoBehaviour
{
    [SerializeField] private GameObject _charaButton;

    private bool _allySelected;

    //Data
    private PlayerSpecs _playerCaptain;
    private MateSpecs _playerAlly;
    private PlayerSpecs _aiCaptain;
    private MateSpecs _aiAlly;

    [HideInInspector] public static int GameTime;
    [HideInInspector] public static float GoalToWin;
    [HideInInspector] public static int AIDifficulty;

    //Chara buttons
    [SerializeField]
    private GameObject _mainCharacterSelection;
    private GameObject _fsMainCharacter;

    [SerializeField]
    private GameObject _allySelection;
    private GameObject _fsAlly;

    [SerializeField]
    private GameObject _matchSettings;
    [SerializeField]
    private GameObject _fsMatchSettings;

    [SerializeField]
    private EventSystem _es;

    [SerializeField] private List<GameObject> _matchParams = new List<GameObject>();

    //Charas
    [SerializeField] private PlayerSpecs _goalSpecs;
    [SerializeField] private List<PlayerSpecs> _maincharaSpecs = new List<PlayerSpecs>();
    [SerializeField] private List<MateSpecs> _mateSpecs = new List<MateSpecs>();
    private List<PlayerSpecs> _onlyMateSpecs = new List<PlayerSpecs>();

    private List<Button> _charaButtons;
    private List<Button> _allyButtons;

    private GameObject _currentSelectedGameObject;

    //Stats
    public TextMeshProUGUI Accuracy;
    public TextMeshProUGUI Speed;
    public TextMeshProUGUI StunTime;
    //

    private static Match_UI_Manager _instance;

    #region Awake/Start/Update

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        _allySelected = false;
        _fsMainCharacter = InitCharacters(_mainCharacterSelection);

        _es.SetSelectedGameObject(_fsMainCharacter);

        //UpdateStat
        _currentSelectedGameObject = _fsMainCharacter;
        UpdateStat(_currentSelectedGameObject);

        //Initialize match params UI
        foreach (var param in _matchParams)
        {
            ISliderValue slider = param.GetComponent<ISliderValue>();

            if (slider != null)
                slider.OnValueChange(param.GetComponentInChildren<Slider>().value);
        }

        //Initialize onlyMateSpecs
        foreach (var elem in _mateSpecs)
        {
            _onlyMateSpecs.Add(elem.MateSpec);
        }
    }

    private void Update()
    {
        //MatchSetting
        if (_matchSettings.activeSelf)
        {
            if ((Keyboard.current?.enterKey.wasPressedThisFrame ?? false)
                || (Gamepad.current?.buttonSouth.wasPressedThisFrame ?? false))
            {
                if (_es.currentSelectedGameObject.TryGetComponent(out Slider sliderGo))
                    _es.SetSelectedGameObject(sliderGo.navigation.selectOnDown.transform.gameObject);
            }
        }

        //UpdateStatChara
        if (_es.currentSelectedGameObject != _currentSelectedGameObject && !_matchSettings.activeSelf)
        {
            _currentSelectedGameObject = _es.currentSelectedGameObject;
            UpdateStat(_currentSelectedGameObject);
        }

        //GoBack
        if ((Keyboard.current?.escapeKey.wasPressedThisFrame ?? false)
            || (Gamepad.current?.buttonEast.wasPressedThisFrame ?? false))
        {
            if (_matchSettings.activeSelf)
            {
                _matchSettings.SetActive(false);
                _allySelection.SetActive(true);
                _es.SetSelectedGameObject(_fsAlly);
            }
            else if (_allySelection.activeSelf)
            {
                _allySelection.SetActive(false);
                _mainCharacterSelection.SetActive(true);
                _es.SetSelectedGameObject(_fsMainCharacter);
            }
            else
                LevelLoader.LoadNextLevel(0);
        }
    }

    #endregion

    private void UpdateStat(GameObject button)
    {
        PlayerSpecs chara;

        if (_allySelection.activeSelf)
            chara = _mateSpecs[_allyButtons.IndexOf(button.GetComponent<Button>())].MateSpec;
        else
            chara = _maincharaSpecs[_charaButtons.IndexOf(button.GetComponent<Button>())];

        Accuracy.text = "Accuracy : " + chara.Accuracy;
        Speed.text = "Speed : " + chara.Speed;
        StunTime.text = "StunTime : " + chara.StunTime;
    }

    GameObject InitCharacters(GameObject Step)
    {
        GridLayoutGroup CharaSection = Step.GetComponentInChildren<GridLayoutGroup>();
        GameObject FS = null;

        List<Button> Buttons = new List<Button>();

        Quaternion rot = Quaternion.Euler(new Vector3(0, 180, 0));
        Vector3 pos = new Vector3(0, -25, -15);
        Vector3 scale = new Vector3(30, 30, 30);

        List<PlayerSpecs> charaspecs;

        if (_allySelection.activeSelf)
        {
            charaspecs = _onlyMateSpecs;
        }
        else
        {
            charaspecs = _maincharaSpecs;
        }

        for (int i = 0; i < charaspecs.Count; i++)
        {
            GameObject newChara = Instantiate(_charaButton, CharaSection.transform);
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

            if (i == 0)
            {
                FS = newChara;
            }
        }

        Navigation btnNav = new Navigation();
        for (int i = 0; i < Buttons.Count; i++)
        {
            btnNav.mode = Navigation.Mode.Explicit;

            if (i == 0)
            {
                btnNav.selectOnRight = Buttons[i + 1];
                btnNav.selectOnLeft = Buttons[Buttons.Count - 1];
            }
            else if (i == Buttons.Count - 1)
            {
                btnNav.selectOnRight = Buttons[0];
                btnNav.selectOnLeft = Buttons[i - 1];
            }
            else
            {
                btnNav.selectOnRight = Buttons[i + 1];
                btnNav.selectOnLeft = Buttons[i - 1];
            }

            Buttons[i].navigation = btnNav;
        }

        if (_allySelection.activeSelf)
        {
            _allyButtons = Buttons;
        }
        else
        {
            _charaButtons = Buttons;
        }

        return FS;
    }

    public void OnSelected()
    {
        AudioManager.PlaySFX(AudioManager.SFXType.ButtonSelected);
    }

    public void OnContinue(PlayerSpecs CharaSpec)
    {
        AudioManager.PlaySFX(AudioManager.SFXType.ButtonClicked);

        if (_allySelection.activeSelf)
        {
            _es.SetSelectedGameObject(_fsMatchSettings);
            _matchSettings.SetActive(true);

            _allySelection.SetActive(false);

            _playerAlly = _mateSpecs[_onlyMateSpecs.IndexOf(CharaSpec)];
        }

        else if (!_allySelected)
        {
            _allySelected = true;

            _allySelection.SetActive(true);

            _fsAlly = InitCharacters(_allySelection);

            _mainCharacterSelection.SetActive(false);
            _es.SetSelectedGameObject(_fsAlly);

            _playerCaptain = CharaSpec;
        }

        else
        {
            _mainCharacterSelection.SetActive(false);
            _allySelection.SetActive(true);
            _es.SetSelectedGameObject(_fsAlly);
            _playerCaptain = CharaSpec;
        }
    }

    public void OnPlay()
    {
        AudioManager.PlaySFX(AudioManager.SFXType.ButtonClicked);

        GetRandomEnemies();

        GameManager.AddMatch(_playerCaptain, _playerAlly.MateSpec, _aiCaptain, _aiAlly.MateSpec, _goalSpecs, GameTime, GoalToWin, AIDifficulty);
        AudioManager.SetCharaAudio(_playerCaptain, _playerAlly.MateSpec, _aiCaptain, _aiAlly.MateSpec);

        LevelLoader.LoadNextLevel(SceneManager.GetActiveScene().buildIndex + 1);

        void GetRandomEnemies()
        {
            List<PlayerSpecs> charas = new List<PlayerSpecs>();

            foreach (var chara in _maincharaSpecs)
            {
                if (chara != _playerCaptain)
                    charas.Add(chara);
            }

            _aiCaptain = charas[Random.Range(0, charas.Count)];


            charas.Clear();

            foreach (var chara in _onlyMateSpecs)
            {
                if (_mateSpecs[_onlyMateSpecs.IndexOf(chara)].Type != _playerAlly.Type)
                    charas.Add(chara);
            }

            _aiAlly = _mateSpecs[_onlyMateSpecs.IndexOf(charas[Random.Range(0, charas.Count)])];
        }
    }
}
