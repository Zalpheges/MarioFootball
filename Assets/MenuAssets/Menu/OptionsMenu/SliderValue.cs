using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SliderValue : MonoBehaviour
{
    [SerializeField]
    private Slider slider;

    [SerializeField]
    private Text text;

    private void Start()
    {
        text.text=(slider.value*100).ToString();
    }
    public void OnValueChange(float Value)
    {
        int newValue = (int)Mathf.Round(Value*100f);  
        text.text = newValue.ToString();
    }

}
