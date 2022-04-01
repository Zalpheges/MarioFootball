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
        if(scoreTeam1 != -1)
            _instance._scoreTeam1.text = $"{_instance.FormatInt(scoreTeam1)}";

        if (scoreTeam2 != -1)
            _instance._scoreTeam2.text = $"{_instance.FormatInt(scoreTeam2)}";
    }

    private string FormatInt(int number)
    {
        return $"{(number < 10 ? "0" : "") + number}";
    }
}