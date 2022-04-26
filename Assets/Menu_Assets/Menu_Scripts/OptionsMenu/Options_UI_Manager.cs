using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

public class Options_UI_Manager : MonoBehaviour
{
    private AudioSource Music;
    [SerializeField]
    private AudioMixer Mixer;

    [SerializeField]
    private GameObject Audio_Options;
    [SerializeField]
    private GameObject FS_Audio_Options;

    [SerializeField]
    private GameObject Controls_Options;
    [SerializeField]
    private GameObject FS_Controls;

    [SerializeField]
    private GameObject FS_Options;

    [SerializeField]
    private EventSystem ES;

    private void Awake()
    {
        Music = AudioManager.GameObject.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if ((Keyboard.current?.escapeKey.wasPressedThisFrame ?? false) || (Gamepad.current?.buttonEast.wasPressedThisFrame ?? false))
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
                LevelLoader.LoadNextLevel(0);
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
        AudioManager.PlaySFX(AudioManager.SFXType.ButtonClicked);
    }

    public void OnControls()
    {
        Controls_Options.SetActive(true);
        ES.SetSelectedGameObject(FS_Controls);
        AudioManager.PlaySFX(AudioManager.SFXType.ButtonClicked);
    }

    public void OnSelected()
    {
        AudioManager.PlaySFX(AudioManager.SFXType.ButtonSelected);
    }

    public void OnClicked()
    {
        AudioManager.PlaySFX(AudioManager.SFXType.ButtonClicked);
    }
}