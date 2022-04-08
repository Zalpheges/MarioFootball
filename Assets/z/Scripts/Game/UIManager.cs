using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;

    [SerializeField] private TextMeshProUGUI _scoreTeam1;
    [SerializeField] private TextMeshProUGUI _scoreTeam2;
    [SerializeField] private TextMeshProUGUI _chrono;
    [SerializeField] private SpriteRenderer _item1Team1;
    [SerializeField] private SpriteRenderer _item2Team1;
    [SerializeField] private SpriteRenderer _item1Team2;
    [SerializeField] private SpriteRenderer _item2Team2;

    private void Awake()
    {
        _instance = this;
    }

    public static void SetChrono(Chrono chrono)
    {
        if (!_instance)
            return;

        _instance._chrono.text = $"{chrono.Minutes}:{chrono.Seconds}";
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

    public static void UpdateItems(Queue<Sprite> items, Team team)
    {
        if (!_instance)
            return;

        Sprite[] itemsArray = items.ToArray();
        bool isTeam1 = team == Field.Team1;
        (isTeam1 ? _instance._item1Team1 : _instance._item1Team2).sprite = itemsArray[0];
        (isTeam1 ? _instance._item2Team1 : _instance._item2Team2).sprite = items.Count > 1 ? itemsArray[1] : null;
    }
}