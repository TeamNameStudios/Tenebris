using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CorruptionBar : MonoBehaviour
{

    public Slider corruptionSlider;

    public void SetMaxValue(float corruptionValue)
    {
        corruptionSlider.maxValue = corruptionValue;
        corruptionSlider.value = corruptionValue;
    }

    public void SetCorruptionBar(float corruptionValue)
    {
        corruptionSlider.value = corruptionValue;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
