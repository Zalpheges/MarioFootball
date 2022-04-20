using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

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

    [SerializeField] private TextMeshProUGUI _endOfGameText;
    [SerializeField] private GameObject _pressToContinue;
    [SerializeField] private Animator _uIAnimator;


    public enum gameState
    {
        Win,
        Loose,
        Draw
    }
    private void Awake()
    {
        _instance = this;
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

    public static void EndOfGame( gameState state)
    {
        if(state == gameState.Win)
        {
            _instance._endOfGameText.text = "YOU WIN";
        }
        else if(state == gameState.Loose)
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
}