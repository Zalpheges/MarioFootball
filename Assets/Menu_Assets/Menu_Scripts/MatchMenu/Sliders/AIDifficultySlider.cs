using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AIDifficultySlider : MonoBehaviour, ISliderValue
{
    [SerializeField]
    private Slider slider;

    [SerializeField]
    private Text text;
    public void OnValueChange(float Value)
    {
        if (Value > 0.66f)
        {
            text.text = "Difficult";
            Match_UI_Manager._instance.AIDifficulty = 3;
        } 
        else if (Value > 0.33f)
        {
            text.text = "Normal";
            Match_UI_Manager._instance.AIDifficulty = 2;
        }
        else
        {
            text.text = "Easy";
            Match_UI_Manager._instance.AIDifficulty = 1;
        }

    }

    
}
