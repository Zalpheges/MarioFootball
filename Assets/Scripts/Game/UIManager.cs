using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager _instance;

    [SerializeField] private TextMeshProUGUI _scoreTeam1;
    [SerializeField] private TextMeshProUGUI _scoreTeam2;
    [SerializeField] private TextMeshProUGUI _chrono;
    [SerializeField] private SpriteRenderer _item1Team1;
    [SerializeField] private SpriteRenderer _item2Team1;
    [SerializeField] private SpriteRenderer _item1Team2;
    [SerializeField] private SpriteRenderer _item2Team2;

    [SerializeField] private EventSystem ES;
    [SerializeField] private GameObject PauseMenu;
    private GameObject FS_PauseMenu;
    [SerializeField] private GameObject SubPauseMenu;
    [SerializeField] private GameObject FS_SubPauseMenu;

    [SerializeField] private TextMeshProUGUI _endOfGameText;
    [SerializeField] private GameObject _pressToContinue;
    [SerializeField] private Animator _uIAnimator;

    [SerializeField] private SpriteRenderer _CornerTeam1;
    [SerializeField] private SpriteRenderer _CornerTeam2;
    [SerializeField] private SpriteRenderer _IconCaptain1;
    [SerializeField] private SpriteRenderer _IconCaptain2;
    [SerializeField] private TextMeshProUGUI _NameCaptain1;
    [SerializeField] private TextMeshProUGUI _NameCaptain2;

    [SerializeField] private TextMeshProUGUI _Announcement;

    private bool _AnnouncementDisplayed;

    private float _timer = 0f;
    private float _announcementDuration = 0f;

    public enum gameState
    {
        Win,
        Loose,
        Draw
    }
    public enum AnnouncementType
    {
        Goal,
        CountDown,
        OneMinuteLeft
    }

    private void Awake()
    {
        FS_PauseMenu = ES.firstSelectedGameObject;
        _instance = this;
    }

    public void InitHUD(PlayerSpecs specsTeam1, PlayerSpecs specsTeam2)
    {
        _CornerTeam1.color = specsTeam1.Color;
        _CornerTeam2.color = specsTeam2.Color;

        _IconCaptain1.sprite = specsTeam1.Icon;
        _IconCaptain2.sprite = specsTeam2.Icon;

        _NameCaptain1.text = specsTeam1.Name;
        _NameCaptain2.text = specsTeam2.Name;
    }

    private void Update()
    {
        if (_chrono.color != Color.red && _chrono.text[0] == '0' && _chrono.text[1] == '0')
        {
            _chrono.color = Color.red;
            DisplayAnnouncement(AnnouncementType.OneMinuteLeft);
        }

        DisplayAnnouncement();

        if ((Keyboard.current?.escapeKey.wasPressedThisFrame ?? false) || (Gamepad.current?.selectButton.wasPressedThisFrame ?? false))
        {
            if (PauseMenu.activeSelf)
                OnGoBack();
            else
            {
                TimeManager.Pause();
                PauseMenu.SetActive(true);
            }
        }

        if (((Gamepad.current?.buttonEast.wasPressedThisFrame ?? false) || (Gamepad.current?.selectButton.wasPressedThisFrame ?? false)) && PauseMenu.activeSelf)
        {
            OnGoBack();
        }
    }

    private void DisplayAnnouncement()
    {
        if (_AnnouncementDisplayed)
        {
            if (_instance._timer < _announcementDuration)
                _instance._timer += Time.deltaTime;
            else
                _AnnouncementDisplayed = false;

            if (_instance._timer < _announcementDuration / 4 || _instance._timer > 3 * _announcementDuration / 4)
            {
                Color color = _Announcement.color;
                float gradient;

                if (_instance._timer < _announcementDuration / 4)
                    gradient = _instance._timer / (_announcementDuration / 4);
                else
                    gradient = 1 - (_instance._timer - 3 * _announcementDuration / 4) / (_announcementDuration / 4);

                _Announcement.color = new Color(color.r, color.g, color.b, Mathf.Lerp(0, 1, gradient));
            }
            _Announcement.gameObject.SetActive(_AnnouncementDisplayed);
        }
    }

    public void DisplayAnnouncement(AnnouncementType type)
    {
        _instance._AnnouncementDisplayed = true;
        _instance._timer = 0f;

        switch (type)
        {
            case AnnouncementType.Goal:
                DisplayGoal();
                break;
            case AnnouncementType.OneMinuteLeft:
                DisplayOneMinuteLeft();
                break;
            default:
                break;
        }
    }

    private static void DisplayOneMinuteLeft()
    {
        _instance._Announcement.text = "One Minute left";
        _instance._Announcement.fontSize = 50;
        _instance._announcementDuration = 3f;
    }

    public static void DisplayGoal()
    {
        _instance._Announcement.text = "GOAL";
        _instance._Announcement.fontSize = 130;
        _instance._announcementDuration = 5f;
    }

    public static void SetChrono(Chrono chrono)
    {
        _instance._chrono.text = $"{_instance.FormatInt(chrono.Minutes)}:{_instance.FormatInt(chrono.Seconds)}";
    }

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

    private string FormatInt(int number)
    {
        return (number < 10 ? "0" : "") + number;
    }

    public static void EndOfGame(gameState state)
    {
        if (state == gameState.Win)
        {
            _instance._endOfGameText.text = "YOU WIN";
        }
        else if (state == gameState.Loose)
        {
            _instance._endOfGameText.text = "YOU LOOSE";
        }
        else
        {
            _instance._endOfGameText.text = "DRAW";
        }

        _instance._uIAnimator.SetTrigger("EndOfGame");

        _instance.StartCoroutine(_instance.waitBeforeContinue());
    }


    IEnumerator waitBeforeContinue()
    {
        yield return new WaitForSeconds(2);
        _pressToContinue.SetActive(true);
    }
    public void OnQuit()
    {
        SubPauseMenu.SetActive(true);
        ES.SetSelectedGameObject(FS_SubPauseMenu);
    }

    public void OnYes()
    {
        LevelLoader.LoadNextLevel(0);
    }

    public void OnGoBack()
    {
        if (SubPauseMenu.activeSelf)
        {
            SubPauseMenu.SetActive(false);
            ES.SetSelectedGameObject(FS_PauseMenu);
        }
        else
        {
            PauseMenu.SetActive(false);
            TimeManager.Play();
        }
    }
}