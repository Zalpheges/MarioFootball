using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MaxGoalSlider: MonoBehaviour, ISliderValue
{
    [SerializeField]
    private Slider slider;

    [SerializeField]
    private Text text;

    int MaxGoal = 10;
    public void OnValueChange(float Value)
    {
        int newValue = (int)Mathf.Round(Value * MaxGoal);
        text.text = newValue.ToString();

        Match_UI_Manager._instance.goalToWin = newValue;
    }
}
