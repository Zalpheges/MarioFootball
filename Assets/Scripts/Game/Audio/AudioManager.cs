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
    [SerializeField] private AudioClip[] _matchMusic;
    [SerializeField] private AudioClip _kickoff;
    [SerializeField] private AudioClip[] _goal;
    [SerializeField] private AudioClip[] _matchOver;
    [SerializeField] private AudioClip _buttonSelected;
    [SerializeField] private AudioClip _buttonClicked;

    [SerializeField] private CharaAudio[] charaAudios;

    [SerializeField] private AudioClip _crowdNomal;
    [SerializeField] private AudioClip _crowdGoal;


    [HideInInspector] public CharaAudio _playerCaptainAudio;
    [HideInInspector] public CharaAudio _playerMateAudio;
    [HideInInspector] public CharaAudio _aiCaptainAudio;
    [HideInInspector] public CharaAudio _aiMateAudio;


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
        ButtonSelected,
        ButtonClicked
    }

    public enum charaSFXType
    {
        Celebrate,
        Electrocuted,
        Shoot,
        Pass,
        ThrowItem
    }

    public enum CrowdSoundType
    {
        Normal,
        Goal
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

        MusicAudioSource = GetComponents<AudioSource>()[0];
        SFXAudioSource = GetComponents<AudioSource>()[1];

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
            int rndIndex = Random.Range(0, _matchMusic.Length);
            clip = _matchMusic[rndIndex];
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
            clip = _kickoff;
        }
        else if(type == SFXType.Goal)
        {
            int rndIndex = Random.Range(0, _goal.Length);
            clip = _goal[rndIndex];
        }
        else if(type==SFXType.MatchOver)
        {
            int rndIndex = Random.Range(0, _matchOver.Length);
            clip = _matchOver[rndIndex];
        }
        else if(type==SFXType.ButtonSelected)
        {
            clip = _buttonSelected;
        }
        else if(type == SFXType.ButtonClicked)
        {
            clip = _buttonClicked;
        }

        if (clip != null)
        {
            SFXAudioSource.clip = clip;
            SFXAudioSource.Play();
        }
    }

    public void PlayCrowdSound(CrowdSoundType type)
    {
        AudioClip clip = null;

        bool isLooping = false;

        if (type == CrowdSoundType.Goal)
        {
            clip = _crowdGoal;
            isLooping = true;
        }
        else
        {
            clip = _crowdNomal;
        }

        if (clip != null)
        {
            GameObject go = new GameObject();
            go.name = clip.name;
            AudioSource AS = go.AddComponent<AudioSource>();
            AS.volume = 0.3f;
            AS.clip = clip;
            AS.Play();

            if (isLooping)
            {
                AS.loop = true;

            }
            else
            {
                Destroy(go, AS.clip.length);
            }
            
            
        }
    }

    public void PlayCharaSFX(charaSFXType SFXtype, CharaAudio Chara)
    {
        AudioClip clip = null;

        if (SFXtype == charaSFXType.Pass)
        {
            if(Chara.pass.Length > 0)
            {
                int rndIndex = Random.Range(0, Chara.pass.Length);
                clip = Chara.pass[rndIndex];
            }
        }
        else if(SFXtype==charaSFXType.Electrocuted)
        {
            if(Chara.electrocuted.Length > 0)
            {
                int rndIndex = Random.Range(0, Chara.electrocuted.Length);
                clip = Chara.electrocuted[rndIndex];
            }
        }
        else if(SFXtype == charaSFXType.Celebrate)
        {
            if(Chara.celebrate.Length > 0)
            {
                int rndIndex = Random.Range(0, Chara.celebrate.Length);
                clip = Chara.celebrate[rndIndex];
            }
        }
        else if(SFXtype == charaSFXType.Shoot)
        {
            if(Chara.shoot.Length > 0)
            {
                int rndIndex = Random.Range(0, Chara.shoot.Length);
                clip = Chara.shoot[rndIndex];
            }
        }
        else if(SFXtype == charaSFXType.ThrowItem)
        {
            if(Chara.throwItem.Length > 0)
            {
                int rndIndex = Random.Range(0, Chara.throwItem.Length);
                clip = Chara.throwItem[rndIndex];
            }
        }

        if (clip != null)
        {
            GameObject go = new GameObject();
            go.name = Chara.name +'-'+ clip.name;
            AudioSource AS = go.AddComponent<AudioSource>();
            AS.outputAudioMixerGroup = SFXAudioSource.outputAudioMixerGroup;
            AS.clip = clip;
            AS.Play();
            Destroy(go, AS.clip.length);
        }
    }

    public void SetCharaAudio(PlayerSpecs playerCaptain, PlayerSpecs playerMate, PlayerSpecs AICaptain, PlayerSpecs AIMate)
    {
        foreach(var audio in charaAudios)
        {
            if (audio.playerLinkedTo == playerCaptain)
                _playerCaptainAudio = audio;
            else if(audio.playerLinkedTo == playerMate)
                _playerMateAudio = audio;
            else if(audio.playerLinkedTo == AICaptain)
                _aiCaptainAudio = audio;
            else if(audio.playerLinkedTo == AIMate)
                _aiMateAudio = audio;
        }
    }
}
