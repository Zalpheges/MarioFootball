using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Global")]
    [SerializeField] private AudioSource _musicAudioSource;
    [SerializeField] private AudioSource _sfxAudioSource;

    [Header("Menu")]
    [SerializeField] private AudioClip _menuMusic;

    [Header("Match")]
    [SerializeField] private AudioClip[] _matchMusic;
    [SerializeField] private AudioClip _kickoff;
    [SerializeField] private AudioClip[] _goal;
    [SerializeField] private AudioClip[] _matchOver;
    [SerializeField] private AudioClip _buttonSelected;
    [SerializeField] private AudioClip _buttonClicked;

    [SerializeField] private CharaAudio[] _charaAudios;

    [SerializeField] private AudioClip _crowdNomal;
    [SerializeField] private AudioClip _crowdGoal;


    [HideInInspector] public static CharaAudio PlayerCaptainAudio;
    [HideInInspector] public static CharaAudio PlayerMateAudio;
    [HideInInspector] public static CharaAudio AiCaptainAudio;
    [HideInInspector] public static CharaAudio AiMateAudio;
    [HideInInspector] public static GameObject GameObject => _instance.gameObject;


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

    public enum CharaSFXType
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

    private static AudioManager _instance;
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(this);
            return;
        }

        AudioSource[] sources = GetComponents<AudioSource>();
        _musicAudioSource = sources[0];
        _sfxAudioSource = sources[1];

        PlayMusic(MusicType.Menu);
    }

    public static void PlayMusic(MusicType type)
    {
        AudioClip clip = null;
        if (type == MusicType.Menu)
        {
            clip = _instance._menuMusic;
        }
        else if (type == MusicType.Match)
        {
            int rndIndex = Random.Range(0, _instance._matchMusic.Length);
            clip = _instance._matchMusic[rndIndex];
        }

        if (clip != null)
        {
            _instance._musicAudioSource.clip = clip;
            _instance._musicAudioSource.loop = true;
            _instance._musicAudioSource.Play();
        }
    }

    public static void PlaySFX(SFXType type)
    {
        AudioClip clip = null;
        if (type == SFXType.Kickoff)
        {
            clip = _instance._kickoff;
        }
        else if (type == SFXType.Goal)
        {
            int rndIndex = Random.Range(0, _instance._goal.Length);
            clip = _instance._goal[rndIndex];
        }
        else if (type == SFXType.MatchOver)
        {
            int rndIndex = Random.Range(0, _instance._matchOver.Length);
            clip = _instance._matchOver[rndIndex];
        }
        else if (type == SFXType.ButtonSelected)
        {
            clip = _instance._buttonSelected;
        }
        else if (type == SFXType.ButtonClicked)
        {
            clip = _instance._buttonClicked;
        }

        if (clip != null)
        {
            _instance._sfxAudioSource.clip = clip;
            _instance._sfxAudioSource.Play();
        }
    }

    public static void PlayCrowdSound(CrowdSoundType type)
    {
        AudioClip clip = null;

        bool isLooping = false;

        if (type == CrowdSoundType.Goal)
        {
            clip = _instance._crowdGoal;
            isLooping = true;
        }
        else
        {
            clip = _instance._crowdNomal;
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

    public static void PlayCharaSFX(CharaSFXType SFXtype, CharaAudio Chara)
    {
        AudioClip clip = null;

        if (SFXtype == CharaSFXType.Pass)
        {
            if (Chara.Pass.Length > 0)
            {
                int rndIndex = Random.Range(0, Chara.Pass.Length);
                clip = Chara.Pass[rndIndex];
            }
        }
        else if (SFXtype == CharaSFXType.Electrocuted)
        {
            if (Chara.Electrocuted.Length > 0)
            {
                int rndIndex = Random.Range(0, Chara.Electrocuted.Length);
                clip = Chara.Electrocuted[rndIndex];
            }
        }
        else if (SFXtype == CharaSFXType.Celebrate)
        {
            if (Chara.Celebrate.Length > 0)
            {
                int rndIndex = Random.Range(0, Chara.Celebrate.Length);
                clip = Chara.Celebrate[rndIndex];
            }
        }
        else if (SFXtype == CharaSFXType.Shoot)
        {
            if (Chara.Shoot.Length > 0)
            {
                int rndIndex = Random.Range(0, Chara.Shoot.Length);
                clip = Chara.Shoot[rndIndex];
            }
        }
        else if (SFXtype == CharaSFXType.ThrowItem)
        {
            if (Chara.ThrowItem.Length > 0)
            {
                int rndIndex = Random.Range(0, Chara.ThrowItem.Length);
                clip = Chara.ThrowItem[rndIndex];
            }
        }

        if (clip != null)
        {
            GameObject go = new GameObject();
            go.name = Chara.Name + '-' + clip.name;
            AudioSource AS = go.AddComponent<AudioSource>();
            AS.outputAudioMixerGroup = _instance._sfxAudioSource.outputAudioMixerGroup;
            AS.clip = clip;
            AS.Play();
            Destroy(go, AS.clip.length);
        }
    }

    public static void SetCharaAudio(PlayerSpecs playerCaptain, PlayerSpecs playerMate, PlayerSpecs AICaptain, PlayerSpecs AIMate)
    {
        foreach (var audio in _instance._charaAudios)
        {
            if (audio.PlayerLinkedTo == playerCaptain)
                PlayerCaptainAudio = audio;
            else if (audio.PlayerLinkedTo == playerMate)
                PlayerMateAudio = audio;
            else if (audio.PlayerLinkedTo == AICaptain)
                AiCaptainAudio = audio;
            else if (audio.PlayerLinkedTo == AIMate)
                AiMateAudio = audio;
        }
    }
}
