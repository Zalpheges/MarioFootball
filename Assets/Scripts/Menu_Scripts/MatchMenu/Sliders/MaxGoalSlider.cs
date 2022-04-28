using UnityEngine;
using UnityEngine.UI;

public class MaxGoalSlider : MonoBehaviour, ISliderValue
{
    [SerializeField]
    private Slider _slider;

    [SerializeField]
    private Text _text;

    private int _maxGoal = 10;
    public void OnValueChange(float Value)
    {
        int newValue = (int)Mathf.Round(Value * _maxGoal);
        _text.text = newValue.ToString();

        Match_UI_Manager.GoalToWin = newValue;
    }
}
