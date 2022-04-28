using UnityEngine;
using UnityEngine.UI;

public class AudioOptionsSlider : MonoBehaviour, ISliderValue
{
    [SerializeField]
    private Slider _slider;

    [SerializeField]
    private Text _text;

    public void OnValueChange(float Value)
    {
        int newValue = Mathf.RoundToInt(Value * 100f);
        _text.text = newValue.ToString();
    }

}
