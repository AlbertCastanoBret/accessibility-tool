using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlSlider : MonoBehaviour
{
    [SerializeField] private Slider slider;

    public void SetSlider(float newValue)
    {
        slider.value = newValue;
    }

    public Slider GetSlider()
    {
        return slider;
    }
}
