using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class Options_UI_Manager : MonoBehaviour
{
    [SerializeField]
    private AudioSource Music;
    [SerializeField]
    private AudioMixer Mixer;

    [SerializeField]
    private GameObject Audio_Options;
    [SerializeField]
    private GameObject Controls_Options;

    [SerializeField]
    private GameObject FS_Options;
    [SerializeField]
    private GameObject FS_Audio_Options;


    [SerializeField]
    private EventSystem ES;

    private void Awake()
    {
        Music = MusicManager._instance.gameObject.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (Audio_Options.activeSelf)
            {
                Audio_Options.SetActive(false);
                ES.SetSelectedGameObject(FS_Options);
            }
            else if (Controls_Options.activeSelf)
            {
                Controls_Options.SetActive(false);
                ES.SetSelectedGameObject(FS_Options);
            }
            else
                SceneManager.LoadScene(0,LoadSceneMode.Single);
        }
    }
    public void OnMusicValueChanged(float Value)
    {
        Music.volume = Value;
    }

    public void OnSFXValueChanged(float Value)
    {
        if (Value < 0.01f)
            Value = 0.01f;

        float volume = Mathf.Log10(Value) * 20;

        Mixer.SetFloat("SFX_Volume", volume);
    }

    public void OnAudio()
    {
        Audio_Options.SetActive(true);
        ES.SetSelectedGameObject(FS_Audio_Options);
    }
}
