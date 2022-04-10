using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEditor;
using System;
using TMPro;

public class Match_UI_Manager : MonoBehaviour
{
    public GameObject charaButton;

    private bool allieSelected;
    private int NbrOfChara;

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
    public List<PlayerSpecs> charaSpecs = new List<PlayerSpecs>();

    List<Button> charaButtons;
    private GameObject actualSelectedGameObject;

    //Stats
    public TextMeshProUGUI accuracy;
    public TextMeshProUGUI speed;
    public TextMeshProUGUI stunTime;
    private void Start()
    {
        NbrOfChara = charaSpecs.Count;

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
    }
    private void Update()
    {
        //UpdateStat
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
                ES.SetSelectedGameObject(FS_allie);
            }
            else if (allieSelection.activeSelf)
            {
                allieSelection.SetActive(false);
                mainCharacterSelection.SetActive(true);
                ES.SetSelectedGameObject(FS_mainCharacter);
            }
            else
                SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
    }

    private void UpdateStat(GameObject button)
    {
        PlayerSpecs chara = charaSpecs[charaButtons.IndexOf(button.GetComponent<Button>())];

        accuracy.text = "Accuracy: "+ chara.Accuracy;
        speed.text = "Speed :" + chara.Speed;
        stunTime.text = "StunTime: " + chara.StunTime;
    }

    GameObject InitCharacters(GameObject Step)
    {
        GridLayoutGroup CharaSection = Step.GetComponentInChildren<GridLayoutGroup>();
        GameObject FS=null;

        charaButtons = new List<Button>();

        for(int i = 0; i<NbrOfChara; i++)
        {
            GameObject newChara = Instantiate(charaButton);

            newChara.transform.SetParent(CharaSection.transform);
            newChara.name = Step.name + "_Chara_"+i.ToString();

            Button btn = newChara.GetComponent<Button>();
            btn.onClick.AddListener(OnContinue);

            charaButtons.Add(btn);

            if(i==0)
            {
                FS = newChara;
            }

        }

        Navigation btnNav = new Navigation();
        for (int i =0; i<charaButtons.Count; i++)
        {
            btnNav.mode = Navigation.Mode.Explicit;

            if(i==0)
            {
                btnNav.selectOnRight = charaButtons[i+1];
                btnNav.selectOnLeft = charaButtons[charaButtons.Count-1];
            }
            else if(i== charaButtons.Count-1)
            {
                btnNav.selectOnRight = charaButtons[0];
                btnNav.selectOnLeft = charaButtons[i-1];
            }
            else
            {
                btnNav.selectOnRight = charaButtons[i + 1];
                btnNav.selectOnLeft = charaButtons[i - 1];
            }
            
            charaButtons[i].navigation = btnNav;
        }

        return FS;
    }
    public void OnContinue()
    {
        if(matchSettings.activeSelf)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
        else if (allieSelection.activeSelf)
        {
            matchSettings.SetActive(true);
            ES.SetSelectedGameObject(FS_matchSettings);
        }
        else if(!allieSelected)
        {
            allieSelected = true;

            FS_allie = InitCharacters(allieSelection);

            mainCharacterSelection.SetActive(false);
            allieSelection.SetActive(true);
            ES.SetSelectedGameObject(FS_allie);
        }
        else
        {
            mainCharacterSelection.SetActive(false);
            allieSelection.SetActive(true);
            ES.SetSelectedGameObject(FS_allie);
        }
    }
}
