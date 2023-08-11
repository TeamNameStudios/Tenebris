using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalLightManager : MonoBehaviour
{
    [SerializeField] GameObject GlobalLight;
    [SerializeField] GameObject GlobalLightCorrupted;

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
            GlobalLight.SetActive(false);
            GlobalLightCorrupted.SetActive(true);
        }
        else
        {
            GlobalLightCorrupted.SetActive(false);
            GlobalLight.SetActive(true);
        }
    }

}
