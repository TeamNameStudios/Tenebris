using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// Insanely basic audio system which supports 3D sound.
/// Ensure you change the 'Sounds' audio source to use 3D spatial blend if you intend to use 3D sounds.
/// </summary>
public class AudioSystem : StaticInstance<AudioSystem> 
{
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource effectsSource;
    [SerializeField] private AudioSource continousEffectsSource;
    [SerializeField] private AudioSource corruptionSource;
    [SerializeField]
    private List<Sound> Effects = new List<Sound>();
    [SerializeField]
    private List<Sound> Musics = new List<Sound>();

    private Shadow tenebris;
    [SerializeField] private float musicDistanceMaxVolume;
    [SerializeField] private float musicMinVolume;

    public AudioMixer audioMixer;
  
    private void OnEnable()
    {
        EventManager<SoundEnum>.Instance.StartListening("onPlayClip", PlayClip);
        EventManager<SoundEnum>.Instance.StartListening("onPlayContinousClip", PlayCountinousClip);
        EventManager<SoundEnum>.Instance.StartListening("onPlayMusic", PlayMusic);
        EventManager<SoundEnum>.Instance.StartListening("onStopContinousClip", StopCountinousClip);
        EventManager<SoundEnum>.Instance.StartListening("onPauseMusic", PauseMusic);
        EventManager<SoundEnum>.Instance.StartListening("onUnPauseMusic", UnPauseMusic);
        EventManager<bool>.Instance.StartListening("onFullyCorrupted", PlayCorruption);
        EventManager<bool>.Instance.StartListening("onPauseAll", PauseAll);
        EventManager<bool>.Instance.StartListening("onUnPauseAll", UnPauseAll);
        EventManager<bool>.Instance.StartListening("onReset", ResetAll);
        EventManager<Shadow>.Instance.StartListening("onSetShadow", SetShadow);
    }

    private void OnDisable()
    {
        EventManager<SoundEnum>.Instance.StopListening("onPlayClip", PlayClip);
        EventManager<SoundEnum>.Instance.StopListening("onPlayContinousClip", PlayClip);
        EventManager<SoundEnum>.Instance.StopListening("onPlayMusic", PlayMusic);
        EventManager<SoundEnum>.Instance.StopListening("onStopContinousClip", StopCountinousClip);
        EventManager<SoundEnum>.Instance.StopListening("onPauseMusic", PauseMusic);
        EventManager<SoundEnum>.Instance.StopListening("onUnPauseMusic", PauseMusic);
        EventManager<bool>.Instance.StopListening("onFullyCorrupted", PlayCorruption);
        EventManager<bool>.Instance.StopListening("onPauseAll", PauseAll);
        EventManager<bool>.Instance.StopListening("onUnPauseAll", UnPauseAll);
        EventManager<bool>.Instance.StopListening("onReset", ResetAll);
        EventManager<Shadow>.Instance.StopListening("onSetShadow", SetShadow);
    }

    private void PlayClip (SoundEnum _soundEnum)
    {
        AudioClip effect = Effects.Find((sound) => { return sound.SoundType == _soundEnum; }).clip;
        effectsSource.PlayOneShot(effect);
    }

    private void PlayCountinousClip(SoundEnum _soundEnum)
    {
        AudioClip effect = Effects.Find((sound) => { return sound.SoundType == _soundEnum; }).clip;
        
        if (continousEffectsSource.clip != effect) {
            continousEffectsSource.clip = effect;
            continousEffectsSource.Play();
        }

    }

    private void PlayMusic(SoundEnum _soundEnum)
    {
        AudioClip music = Musics.Find((sound) => { return sound.SoundType == _soundEnum; }).clip;
        musicSource.clip = music;
        musicSource.Play();
    }

    private void PlayCorruption(bool isCorrupted)
    {
        AudioClip effect = Effects.Find((sound) => { return sound.SoundType == SoundEnum.corruptionSound; }).clip;
        
        if (isCorrupted && corruptionSource.clip != effect)
        {
            corruptionSource.clip = effect;
            corruptionSource.Play();
        }
        else if(!isCorrupted)
        {
            corruptionSource.clip = null;
            corruptionSource.Stop();
        }
        
    }
    private void StopCountinousClip(SoundEnum _soundEnum)
    {
        continousEffectsSource.clip = null;
        continousEffectsSource.Stop();
    }

    private void PauseMusic(SoundEnum _soundEnum)
    {
        musicSource.Pause();

    }

    private void UnPauseMusic(SoundEnum _soundEnum)
    {
        musicSource.UnPause();
    }

    private void PauseAll(bool isPausing)
    {
        musicSource.Pause(); 
        continousEffectsSource.Pause();
        corruptionSource.Pause();
    }

    private void UnPauseAll(bool isUnPausing)
    {
        musicSource.UnPause();
        continousEffectsSource.UnPause();
        corruptionSource.UnPause();
    }


    private void ResetAll(bool isReset)
    {
        musicSource.clip = null;
        musicSource.Stop();
        continousEffectsSource.clip = null;
        continousEffectsSource.Stop();
        corruptionSource.clip = null;
        corruptionSource.Stop();
    }


    private void Update()
    {
        if (tenebris != null)
        {
            if (tenebris.Distance > musicDistanceMaxVolume)
            {
                float volume = 1 / (tenebris.Distance - musicDistanceMaxVolume) * 10;
                musicSource.volume = volume;
                if (musicSource.volume <= musicMinVolume)
                {
                    musicSource.volume = musicMinVolume;
                }
            }
            else
            {
                musicSource.volume = 1;
            }
        }
    }

    private void SetShadow(Shadow shadow)
    {
        tenebris = shadow;
    }
}

[Serializable]
public struct Sound
{
    public SoundEnum SoundType;
    public AudioClip clip;
}

public enum SoundEnum {
    /*Effects*/
    runSound,
    jumpSound,
    corruptionSound,
    deadSound,
    hitSound,
    collectible1Sound,
    collectible2Sound,
    collectible3Sound,
    dashSound,
    grappleSound,
    gameOverSound,
    chaserSound,
    lurkerSound,
    runnerSound,
    /*Music*/
    mainMenuMusic,
    gameMusic,
}

