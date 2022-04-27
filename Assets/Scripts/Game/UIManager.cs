using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;

    public enum GameState
    {
        Win,
        Loose,
        Draw
    }

    public enum AnnouncementType
    {
        Goal,
        CountDown,
        OneMinuteLeft,
        ReadySetGo
    }

    [SerializeField] private TextMeshProUGUI _scoreTeam1;
    [SerializeField] private TextMeshProUGUI _scoreTeam2;
    [SerializeField] private TextMeshProUGUI _chrono;
    [SerializeField] private SpriteRenderer _item1Team1;
    [SerializeField] private SpriteRenderer _item2Team1;
    [SerializeField] private SpriteRenderer _item1Team2;
    [SerializeField] private SpriteRenderer _item2Team2;

    [SerializeField] private EventSystem _es;
    public static EventSystem ES => _instance._es;
    [SerializeField] private GameObject _pauseMenu;
    [SerializeField] private GameObject _subPauseMenu;
    [SerializeField] private GameObject _fsSubPauseMenu;
    private GameObject _fsPauseMenu;

    [SerializeField] private TextMeshProUGUI _endOfGameText;
    [SerializeField] private GameObject _pressToContinue;
    [SerializeField] private Animator _uIAnimator;

    [SerializeField] private SpriteRenderer _cornerTeam1;
    [SerializeField] private SpriteRenderer _cornerTeam2;
    [SerializeField] private SpriteRenderer _iconCaptain1;
    [SerializeField] private SpriteRenderer _iconCaptain2;
    [SerializeField] private TextMeshProUGUI _nameCaptain1;
    [SerializeField] private TextMeshProUGUI _nameCaptain2;

    [SerializeField] private TextMeshProUGUI _announcement;
    [SerializeField] private TextMeshProUGUI _skipMessage;

    private bool _announcementDisplayed;
    private bool _timerReset = false;

    private float _timer = 0f;
    private float _currentTimer = 0f;

    private List<int> _announcementFontSize = new List<int>();
    private List<string> _announcementContent = new List<string>();
    private List<float> _announcementDuration = new List<float>();

    #region Specific Events

    public static void DisplayAnnouncement(AnnouncementType type)
    {
        _instance._announcementDisplayed = true;
        _instance._timerReset = true;

        switch (type)
        {
            case AnnouncementType.Goal:
                DisplayGoal();
                break;
            case AnnouncementType.OneMinuteLeft:
                DisplayOneMinuteLeft();
                break;
            case AnnouncementType.ReadySetGo:
                DisplayReadySetGo();
                break;
            default:
                break;
        }

        #region Local functions

        static void DisplayGoal()
        {
            _instance._announcementFontSize.Add(130);

            _instance._announcementContent.Add("GOAL");

            _instance._announcementDuration.Add(5f);
        }

        static void DisplayOneMinuteLeft()
        {
            _instance._announcementFontSize.Add(50);

            _instance._announcementContent.Add("One Minute left");

            _instance._announcementDuration.Add(3f);
        }

        static void DisplayReadySetGo()
        {
            _instance._announcementFontSize.Add(100);
            _instance._announcementFontSize.Add(100);
            _instance._announcementFontSize.Add(100);
            _instance._announcementFontSize.Add(100);

            _instance._announcementContent.Add("3");
            _instance._announcementContent.Add("2");
            _instance._announcementContent.Add("1");
            _instance._announcementContent.Add("GO");

            _instance._announcementDuration.Add(1.6f);
            _instance._announcementDuration.Add(1.6f);
            _instance._announcementDuration.Add(1.6f);
            _instance._announcementDuration.Add(1.6f);
        }

        #endregion

    }

    public static void EndOfGame(GameState state)
    {
        if (state == GameState.Win)
        {
            _instance._endOfGameText.text = "YOU WIN";
        }
        else if (state == GameState.Loose)
        {
            _instance._endOfGameText.text = "YOU LOOSE";
        }
        else
        {
            _instance._endOfGameText.text = "DRAW";
        }

        _instance._uIAnimator.SetTrigger("EndOfGame");

        _instance.StartCoroutine(WaitBeforeContinue());

        static IEnumerator WaitBeforeContinue()
        {
            yield return new WaitForSeconds(2);
            _instance._pressToContinue.SetActive(true);
        }
    }

    private void DisplayAnnouncement()
    {
        if (_instance._announcementDisplayed)
        {
            if (_timerReset)
            {
                _instance._timerReset = false;
                _instance._timer = 0f;
                _currentTimer = _announcementDuration[0];
                _instance._announcement.fontSize = _instance._announcementFontSize[0];
                _instance._announcement.text = _instance._announcementContent[0];
            }
            else if (_instance._timer < _currentTimer)
            {
                _instance._timer += Time.deltaTime;

                if (_instance._timer < _currentTimer / 4 || _instance._timer > 3 * _currentTimer / 4)
                {
                    Color color = _announcement.color;
                    float gradient;

                    if (_instance._timer < _currentTimer / 4)
                        gradient = _instance._timer / (_currentTimer / 4);
                    else
                        gradient = 1 - (_instance._timer - 3 * _currentTimer / 4) / (_currentTimer / 4);

                    _announcement.color = new Color(color.r, color.g, color.b, Mathf.Lerp(0, 1, gradient));
                }
            }
            else
            {
                _instance._announcementFontSize.RemoveAt(0);
                _instance._announcementContent.RemoveAt(0);
                _instance._announcementDuration.RemoveAt(0);

                if (_instance._announcementContent.Count == 0)
                    _instance._announcementDisplayed = false;
                else
                    _instance._timerReset = true;
            }

            _announcement.gameObject.SetActive(_instance._announcementDisplayed);
        }
    }

    public static void SkipAnnouncement()
    {
        if (_instance._announcementDisplayed)
        {
            _instance._announcementFontSize.RemoveAt(0);
            _instance._announcementContent.RemoveAt(0);
            _instance._announcementDuration.RemoveAt(0);

            if (_instance._announcementContent.Count == 0)
                _instance._announcementDisplayed = false;
            else
                _instance._timerReset = true;
        }
    }

    #endregion

    #region Awake/Update

    private void Awake()
    {
        _fsPauseMenu = _es.firstSelectedGameObject;
        _instance = this;
    }

    private void Update()
    {
        _skipMessage.gameObject.SetActive(GameManager.CanSkip);
        if (_skipMessage.gameObject.activeSelf)
        {
            _skipMessage.text = $"Press {Keyboard.current?.enterKey.name ?? Gamepad.current?.startButton.name ?? ""} to skip";
        }
        if (_chrono.color != Color.red && _chrono.text[0] == '0' && _chrono.text[1] == '0')
        {
            _chrono.color = Color.red;
            DisplayAnnouncement(AnnouncementType.OneMinuteLeft);
        }
        DisplayAnnouncement();

        if ((Keyboard.current?.escapeKey.wasPressedThisFrame ?? false)
            || (Gamepad.current?.selectButton.wasPressedThisFrame ?? false))
        {
            if (_pauseMenu.activeSelf)
                OnGoBack();
            else
            {
                TimeManager.Pause();
                _pauseMenu.SetActive(true);
            }
        }

        if (((Gamepad.current?.buttonEast.wasPressedThisFrame ?? false)
            || (Gamepad.current?.selectButton.wasPressedThisFrame ?? false)) && _pauseMenu.activeSelf)
        {
            OnGoBack();
        }
    }

    #endregion

    #region Update Infos

    public static void SetScore(int scoreTeam1 = -1, int scoreTeam2 = -1)
    {
        if (!_instance)
            return;

        if (scoreTeam1 != -1)
            _instance._scoreTeam1.text = $"{scoreTeam1}";

        if (scoreTeam2 != -1)
            _instance._scoreTeam2.text = $"{scoreTeam2}";
    }

    public static void UpdateItems(Queue<ItemData> items, Team team)
    {
        if (!_instance)
            return;

        ItemData[] itemsArray = items.ToArray();
        bool isTeam1 = team == Field.Team1;
        (isTeam1 ? _instance._item1Team1 : _instance._item1Team2).sprite = items.Count > 0 ? itemsArray[0].Sprite : null;
        (isTeam1 ? _instance._item2Team1 : _instance._item2Team2).sprite = items.Count > 1 ? itemsArray[1].Sprite : null;
    }

    public static void InitHUD(PlayerSpecs specsTeam1, PlayerSpecs specsTeam2)
    {
        _instance._cornerTeam1.color = specsTeam1.Color;
        _instance._cornerTeam2.color = specsTeam2.Color;

        _instance._iconCaptain1.sprite = specsTeam1.Icon;
        _instance._iconCaptain2.sprite = specsTeam2.Icon;

        _instance._nameCaptain1.text = specsTeam1.Name;
        _instance._nameCaptain2.text = specsTeam2.Name;
    }

    public static void SetChrono(Chrono chrono)
    {
        _instance._chrono.text = $"{_instance.FormatInt(chrono.Minutes)}:{_instance.FormatInt(chrono.Seconds)}";
    }

    #endregion

    #region Utility

    private string FormatInt(int number)
    {
        return (number < 10 ? "0" : "") + number;
    }

    #endregion

    #region OnInteraction

    public void OnQuit()
    {
        AudioManager.PlaySFX(AudioManager.SFXType.ButtonClicked);
        _subPauseMenu.SetActive(true);
        _es.SetSelectedGameObject(_fsSubPauseMenu);
    }

    public void OnGoBack()
    {
        AudioManager.PlaySFX(AudioManager.SFXType.ButtonClicked);
        if (_subPauseMenu.activeSelf)
        {
            _subPauseMenu.SetActive(false);
            _es.SetSelectedGameObject(_fsPauseMenu);
        }
        else
        {
            _pauseMenu.SetActive(false);
            TimeManager.Play();
        }
    }
    public void OnYes()
    {
        AudioManager.PlaySFX(AudioManager.SFXType.ButtonClicked);
        LevelLoader.LoadNextLevel(0);
    }

    public void OnSelected()
    {
        AudioManager.PlaySFX(AudioManager.SFXType.ButtonSelected);
    }

    #endregion

}