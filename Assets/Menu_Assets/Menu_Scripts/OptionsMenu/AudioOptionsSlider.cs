using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioOptionsSlider : MonoBehaviour,ISliderValue
{
    [SerializeField]
    private Slider slider;

    [SerializeField]
    private Text text;

    public void OnValueChange(float Value)
    {
        int newValue = (int)Mathf.Round(Value*100f);  
        text.text = newValue.ToString();
    }

}
