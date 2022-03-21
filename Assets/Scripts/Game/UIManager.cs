using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager _instance;

    [SerializeField] private TextMeshProUGUI _scoreEquipe1;
    [SerializeField] private TextMeshProUGUI _scoreEquipe2;
    [SerializeField] private TextMeshProUGUI _chrono;

    private void Awake()
    {
        _instance = this;
    }

    public static void SetChrono(Chrono chrono)
    {
        _instance._chrono.text = $"{_instance.FormatInt(chrono.Minutes)}:{_instance.FormatInt(chrono.Seconds)}";
    }

    public static void SetScore(int scoreEquipe1, int scoreEquipe2)
    {
        _instance._scoreEquipe1.text = $"{_instance.FormatInt(scoreEquipe1)}";
        _instance._scoreEquipe2.text = $"{_instance.FormatInt(scoreEquipe2)}";
    }

    private string FormatInt(int number)
    {
        return $"{(number < 10 ? "0" : "") + number}";
    }
}