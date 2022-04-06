using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEditor;

public class Match_UI_Manager : MonoBehaviour
{
    public GameObject charaButton;
    public int NbrOfChara=4;

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


    private void Start()
    {
        FS_mainCharacter=InitCharacters(mainCharacterSelection);
        ES.SetSelectedGameObject(FS_mainCharacter);
    }
    private void Update()
    {
        //GoBack
        if ((Keyboard.current?.escapeKey.wasPressedThisFrame ?? false) || (Gamepad.current?.buttonEast.wasPressedThisFrame ?? false))
        {
            if (allieSelection.activeSelf)
            {
                FS_mainCharacter=InitCharacters(mainCharacterSelection);

                allieSelection.SetActive(false);
                ES.SetSelectedGameObject(FS_mainCharacter);
            }
            else if (matchSettings.activeSelf)
            {
                FS_allie=InitCharacters(allieSelection);

                matchSettings.SetActive(false);
                ES.SetSelectedGameObject(FS_allie);
            }
            else
                SceneManager.LoadScene(0, LoadSceneMode.Single);
        }
    }
    
    GameObject InitCharacters(GameObject Step)
    {
        GridLayoutGroup CharaSection = Step.GetComponentInChildren<GridLayoutGroup>();
        GameObject FS=null;

        List<Button> charaButtons = new List<Button>();

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
        else
        {
            FS_allie = InitCharacters(allieSelection);

            allieSelection.SetActive(true);
            ES.SetSelectedGameObject(FS_allie);
        }
    }
}
