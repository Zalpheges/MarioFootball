using UnityEngine;
using UnityEngine.UI;

public class AIDifficultySlider : MonoBehaviour, ISliderValue
{
    [SerializeField]
    private Slider _slider;

    [SerializeField]
    private Text _text;
    public void OnValueChange(float Value)
    {
        if (Value > 1)
        {
            _text.text = "Difficult";
            Match_UI_Manager.AIDifficulty = 3;
        }
        else if (Value == 1)
        {
            _text.text = "Normal";
            Match_UI_Manager.AIDifficulty = 2;
        }
        else
        {
            _text.text = "Easy";
            Match_UI_Manager.AIDifficulty = 1;
        }

    }

}
