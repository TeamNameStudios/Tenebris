using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsHUD : MonoBehaviour
{
    [SerializeField]
    AudioMixer audioMixer;
    [SerializeField]    
    private List<Slider> sliders = new List<Slider>();
    private void OnEnable()
    {
        EventManager<float>.Instance.StartListening(VolumeType.MASTER.ToString(), ChangeMasterVolume);
        EventManager<float>.Instance.StartListening(VolumeType.MUSIC.ToString(), ChangeMusicVolume);
        EventManager<float>.Instance.StartListening(VolumeType.EFFECTS.ToString(), ChangeEffectVolume);
        EventManager<List<float>>.Instance.StartListening("onLoadAudioData", SetAudioSlider);
        EventManager<bool>.Instance.TriggerEvent("LoadAudioData", true);
    }

    private void OnDisable()
    {
        EventManager<float>.Instance.StopListening(VolumeType.MASTER.ToString(), ChangeMasterVolume);
        EventManager<float>.Instance.StopListening(VolumeType.MUSIC.ToString(), ChangeMusicVolume);
        EventManager<float>.Instance.StopListening(VolumeType.EFFECTS.ToString(), ChangeEffectVolume);
        EventManager<List<float>>.Instance.StopListening("onLoadAudioData", SetAudioSlider);
    }
    private void SetAudioSlider(List<float> audioVolume) {

        sliders[0].value = audioVolume[0];
        sliders[1].value = audioVolume[1];
        sliders[2].value = audioVolume[2];
    }

    public void ChangeMasterVolume(float volume)
    {
        float masterVolume = (1 - volume) * -80;
        audioMixer.SetFloat("MasterVolume", masterVolume);
        EventManager<float>.Instance.TriggerEvent("SaveMasterVolume", volume);
    }

    public void ChangeMusicVolume(float volume)
    {
        float musicVolume = (1 - volume) * -80;
        audioMixer.SetFloat("MusicVolume", musicVolume);
        EventManager<float>.Instance.TriggerEvent("SaveMusicVolume", volume);
    }

    public void ChangeEffectVolume(float volume)
    {
        float effectVolume = (1 - volume) * -80;
        audioMixer.SetFloat("EffectVolume", effectVolume);
        EventManager<float>.Instance.TriggerEvent("SaveEffectVolume", volume);
    }
}

public enum VolumeType { 
    MASTER,
    MUSIC,
    EFFECTS
}
