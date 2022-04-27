using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Options_UI_Manager : MonoBehaviour
{
    private AudioSource _music;
    [SerializeField]
    private AudioMixer _mixer;

    [SerializeField]
    private GameObject _audioOptions;
    [SerializeField]
    private GameObject _fsAudioOptions;

    [SerializeField]
    private GameObject _controlsOptions;
    [SerializeField]
    private GameObject _fsControls;

    [SerializeField]
    private GameObject _fsOptions;

    [SerializeField]
    private EventSystem _es;

    private void Awake()
    {
        _music = AudioManager.GameObject.GetComponent<AudioSource>();
    }

    private void Update()
    {
        if ((Keyboard.current?.escapeKey.wasPressedThisFrame ?? false)
            || (Gamepad.current?.buttonEast.wasPressedThisFrame ?? false))
        {
            if (_audioOptions.activeSelf)
            {
                _audioOptions.SetActive(false);
                _es.SetSelectedGameObject(_fsOptions);
            }
            else if (_controlsOptions.activeSelf)
            {
                _controlsOptions.SetActive(false);
                _es.SetSelectedGameObject(_fsOptions);
            }
            else
                LevelLoader.LoadNextLevel(0);
        }
    }
    public void OnMusicValueChanged(float Value)
    {
        _music.volume = Value;
    }

    public void OnSFXValueChanged(float Value)
    {
        if (Value < 0.01f)
            Value = 0.01f;

        float volume = Mathf.Log10(Value) * 20;

        _mixer.SetFloat("SFX_Volume", volume);
    }

    public void OnAudio()
    {
        _audioOptions.SetActive(true);
        _es.SetSelectedGameObject(_fsAudioOptions);
        AudioManager.PlaySFX(AudioManager.SFXType.ButtonClicked);
    }

    public void OnControls()
    {
        _controlsOptions.SetActive(true);
        _es.SetSelectedGameObject(_fsControls);
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