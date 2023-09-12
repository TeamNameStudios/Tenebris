using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsHUD : MonoBehaviour
{
    private void OnEnable()
    {
        EventManager<float>.Instance.StartListening(VolumeType.MASTER.ToString(), ChangeMasterVolume);
        EventManager<float>.Instance.StartListening(VolumeType.MUSIC.ToString(), ChangeMusicVolume);
        EventManager<float>.Instance.StartListening(VolumeType.EFFECTS.ToString(), ChangeEffectVolume);
    }

    private void OnDisable()
    {
        EventManager<float>.Instance.StopListening(VolumeType.MASTER.ToString(), ChangeMasterVolume);
        EventManager<float>.Instance.StopListening(VolumeType.MUSIC.ToString(), ChangeMusicVolume);
        EventManager<float>.Instance.StopListening(VolumeType.EFFECTS.ToString(), ChangeEffectVolume);
    }

    public void ChangeMasterVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void ChangeMusicVolume(float volume)
    {
        AudioListener.volume = volume;
    }

    public void ChangeEffectVolume(float volume)
    {
        AudioListener.volume = volume;
    }
}

public enum VolumeType { 
    MASTER,
    MUSIC,
    EFFECTS
}
