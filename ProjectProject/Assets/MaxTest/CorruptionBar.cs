using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CorruptionBar : MonoBehaviour
{

    public Slider corruptionSlider;
    public float maxCorruptionBar;

    public void SetMaxValue(float corruptionValue)
    {
        corruptionSlider.maxValue = corruptionValue;
        corruptionSlider.value = corruptionValue;
        EventManager<float>.Instance.StopListening("InitCorruptionBar", SetMaxValue);
    }

    public void SetCorruptionBar(float corruptionValue)
    {
        corruptionSlider.value = corruptionValue;
    }

    private void OnEnable()
    {
        EventManager<float>.Instance.StartListening("UpdateCorruptionBar", SetCorruptionBar);
        EventManager<float>.Instance.StartListening("InitCorruptionBar", SetMaxValue);
    }

    private void OnDisable()
    {
        EventManager<float>.Instance.StopListening("UpdateCorruptionBar", SetCorruptionBar);
        EventManager<float>.Instance.StopListening("InitCorruptionBar", SetMaxValue);
    }
}
