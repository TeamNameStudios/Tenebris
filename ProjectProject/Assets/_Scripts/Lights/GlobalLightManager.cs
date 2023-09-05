using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class GlobalLightManager : MonoBehaviour
{
    [SerializeField] GameObject GlobalLightBackground;
    [SerializeField] GameObject GlobalLightLevel;
    [SerializeField] GameObject GlobalLightCorrupted;
    [SerializeField] GameObject MoonGlobalLight;

    [SerializeField] GameObject Moon;
    [SerializeField] GameObject CorruptedMoon;

    private void Awake()
    {
        Moon.SetActive(true);
        CorruptedMoon.SetActive(false);
   
    }

    private void OnEnable()
    {
        EventManager<bool>.Instance.StartListening("onFullyCorrupted", ChangeGlobalLight);
    }

    private void OnDisable()
    {
        EventManager<bool>.Instance.StopListening("onFullyCorrupted", ChangeGlobalLight);
    }

    private void ChangeGlobalLight(bool value)
    {

        if (value)
        {
            Moon.SetActive(false);
            MoonGlobalLight.SetActive(false);
            GlobalLightBackground.SetActive(false);
            GlobalLightLevel.SetActive(false);
            GlobalLightCorrupted.SetActive(true);
            CorruptedMoon.SetActive(true);
        }
        else
        {
            CorruptedMoon.SetActive(false);
            GlobalLightCorrupted.SetActive(false);
            GlobalLightLevel.SetActive(true);
            GlobalLightBackground.SetActive(true);
            Moon.SetActive(true);
            MoonGlobalLight.SetActive(true);
        }
    }
}
