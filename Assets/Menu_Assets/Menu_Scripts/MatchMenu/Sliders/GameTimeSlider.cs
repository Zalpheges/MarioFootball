using UnityEngine;
using UnityEngine.UI;

public class GameTimeSlider : MonoBehaviour, ISliderValue
{
    [SerializeField]
    private Slider _slider;

    [SerializeField]
    private Text _text;

    private int _maxTime = 10;
    public void OnValueChange(float Value)
    {
        int minutes = Mathf.FloorToInt(Value * _maxTime);
        int seconds = Mathf.RoundToInt((Value * _maxTime - minutes) * 60);

        _text.text = minutes.ToString() + " minutes";

        Match_UI_Manager.GameTime = minutes;
    }
}
