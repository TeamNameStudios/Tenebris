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
    [SerializeField] private float tenebrisMusicMinVolume;
    [SerializeField] private float tenebrisMusicMaxVolume;

    public AudioMixer audioMixer;
    public SoundEnum SoundToTest;
    public SoundEnum MusicToTest;

    private float fadeDuration = 2;
    private float elapsedTime = 0;
    private float musicVolume;
    private Coroutine FadeOutCoroutine;

    private void OnEnable()
    {
        EventManager<SoundEnum>.Instance.StartListening("onPlayClip", PlayClip);
        EventManager<SoundEnum>.Instance.StartListening("onPlayContinousClip", PlayCountinousClip);
        EventManager<SoundEnum>.Instance.StartListening("onPlayMusic", PlayMusic);
        EventManager<bool>.Instance.StartListening("onStopMusic", StopMusic);
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
        EventManager<bool>.Instance.StopListening("onStopMusic", StopMusic);
        EventManager<SoundEnum>.Instance.StopListening("onStopContinousClip", StopCountinousClip);
        EventManager<SoundEnum>.Instance.StopListening("onPauseMusic", PauseMusic);
        EventManager<SoundEnum>.Instance.StopListening("onUnPauseMusic", PauseMusic);
        EventManager<bool>.Instance.StopListening("onFullyCorrupted", PlayCorruption);
        EventManager<bool>.Instance.StopListening("onPauseAll", PauseAll);
        EventManager<bool>.Instance.StopListening("onUnPauseAll", UnPauseAll);
        EventManager<bool>.Instance.StopListening("onReset", ResetAll);
        EventManager<Shadow>.Instance.StopListening("onSetShadow", SetShadow);
    }

    protected override void Awake()
    {
        base.Awake();
        PlayMusic(SoundEnum.mainMenuMusic);
    }

    public void PlayClip (SoundEnum _soundEnum)
    {
        Sound effect = Effects.Find((sound) => { return sound.SoundType == _soundEnum; });
        effectsSource.PlayOneShot(effect.clip, effect.volume);
    }

    private void PlayCountinousClip(SoundEnum _soundEnum)
    {
        Sound effect = Effects.Find((sound) => { return sound.SoundType == _soundEnum; });
        
        if (continousEffectsSource != null)
        {
            if (continousEffectsSource.clip != effect.clip) {
                continousEffectsSource.clip = effect.clip;
                continousEffectsSource.volume = effect.volume;
                continousEffectsSource.Play();
            }
        }
    }

    public void PlayMusic(SoundEnum _soundEnum)
    {
        Sound music = Musics.Find((sound) => { return sound.SoundType == _soundEnum; });
        musicSource.clip = music.clip;
        musicSource.volume = music.volume;
        musicVolume = music.volume;
        musicSource.Play();
        elapsedTime = 0;
    }

    public void StopMusic(bool value)
    {
        FadeOutCoroutine = StartCoroutine(FadeOutMusic());
    }

    private IEnumerator FadeOutMusic()
    {
        while (elapsedTime < fadeDuration)
        {
            musicSource.volume = Mathf.Lerp(musicVolume, 0, elapsedTime / fadeDuration);
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }
        musicSource.Stop();
        yield break;
    }

    private void PlayCorruption(bool isCorrupted)
    {
        Sound effect = Effects.Find((sound) => { return sound.SoundType == SoundEnum.corruptionSound;});
        
        if (isCorrupted && corruptionSource.clip != effect.clip)
        {
            corruptionSource.clip = effect.clip;
            corruptionSource.volume = effect.volume;
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
        elapsedTime = 0;
    }

    private void Update()
    {
        if (tenebris != null)
        {
            if (tenebris.Distance > musicDistanceMaxVolume)
            {
                float volume = 1 / (tenebris.Distance - musicDistanceMaxVolume) * 10;
                musicSource.volume = volume;
                if (musicSource.volume <= tenebrisMusicMinVolume)
                {
                    musicSource.volume = tenebrisMusicMinVolume;
                }
            }
            else
            {
                musicSource.volume = tenebrisMusicMaxVolume;
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
    [Range(0f, 1f)]
    public float volume;
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
    buttonSound,
    /*Music*/
    mainMenuMusic,
    gameMusic,
    mouseHoveringSound,
    tutorialMusic,
}

