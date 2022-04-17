using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameTimeSlider : MonoBehaviour, ISliderValue
{
    [SerializeField]
    private Slider slider;

    [SerializeField]
    private Text text;

    int MaxTime = 10;
    public void OnValueChange(float Value)
    {
        int minutes = (int)Mathf.Floor(Value * MaxTime);
        int seconds = (int)Mathf.Round(((Value * MaxTime) - minutes) * 60);

        text.text = minutes.ToString() + ":" + seconds.ToString("00");

        Match_UI_Manager._instance.gameTime = minutes + seconds;
    }
}
