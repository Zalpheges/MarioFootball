using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    [Header("Global")]
    public AudioSource MusicAudioSource;
    public AudioSource SFXAudioSource;
    [Header("Menu")]
    [SerializeField] private AudioClip MenuMusic;
    [Header("Match")]
    [SerializeField] private AudioClip[] MatchMusic;
    [SerializeField] private AudioClip Kickoff;
    [SerializeField] private AudioClip[] Goal;
    [SerializeField] private AudioClip[] MatchOver;


    public enum MusicType
    {
        Menu,
        Match,
    }
    public enum SFXType
    {
        Kickoff,
        Goal,
        MatchOver,
    }

    public static AudioManager _instance;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
            Destroy(this);

        PlayMusic(MusicType.Menu);
    }

    public void PlayMusic(MusicType type)
    {
        AudioClip clip = null;
        if (type == MusicType.Menu)
        {
            clip = MenuMusic;
        }
        else if (type == MusicType.Match)
        {
            int rndIndex = Random.Range(0, MatchMusic.Length);
            clip = MatchMusic[rndIndex];
        }

        if (clip != null)
        {
            MusicAudioSource.clip = clip;
            MusicAudioSource.loop = true;
            MusicAudioSource.Play();
        }
    }

    public void PlaySFX(SFXType type)
    {
        AudioClip clip = null;
        if(type == SFXType.Kickoff)
        {
            clip = Kickoff;
        }
        else if(type == SFXType.Goal)
        {
            int rndIndex = Random.Range(0, Goal.Length);
            clip = Goal[rndIndex];
        }
        else if(type==SFXType.MatchOver)
        {
            int rndIndex = Random.Range(0, MatchOver.Length);
            clip = MatchOver[rndIndex];
        }

        if(clip != null)
        {
            SFXAudioSource.clip = clip;
            SFXAudioSource.Play();
        }
    }
}
