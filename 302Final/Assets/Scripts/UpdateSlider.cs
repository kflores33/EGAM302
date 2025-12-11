using UnityEngine;
using UnityEngine.UI;

public class UpdateSlider : MonoBehaviour
{
    public Slider slider;

    public void UpdateSliderValue(int newValue, int maxValue)
    {
        slider.minValue = 0;
        slider.maxValue = maxValue;

        slider.value = newValue;
    }
}
