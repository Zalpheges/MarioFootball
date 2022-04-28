using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Menu_UI_Manager : MonoBehaviour
{
    [SerializeField]
    private GameObject Success;

    private void Update()
    {
        if ((Keyboard.current?.escapeKey.wasPressedThisFrame ?? false)
            || (Gamepad.current?.buttonEast.wasPressedThisFrame ?? false))
        {
            if (Success != null && SceneManager.GetActiveScene().buildIndex == 0)
            {
                if (Success.activeSelf)
                    Success.SetActive(false);
                else
                    Application.Quit();
            }
            else
                LevelLoader.LoadNextLevel(SceneManager.GetActiveScene().buildIndex - 1);
        }
    }
    public void OnSuccess()
    {
        Success.SetActive(true);
        AudioManager.PlaySFX(AudioManager.SFXType.ButtonClicked);
    }

    public void OnOptions()
    {
        LevelLoader.LoadNextLevel(3);
        AudioManager.PlaySFX(AudioManager.SFXType.ButtonClicked);
    }

    public void OnContinue()
    {
        LevelLoader.LoadNextLevel(SceneManager.GetActiveScene().buildIndex + 1);
        AudioManager.PlaySFX(AudioManager.SFXType.ButtonClicked);
    }

    public void OnSelected()
    {
        AudioManager.PlaySFX(AudioManager.SFXType.ButtonSelected);
    }

}
