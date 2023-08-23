using System;
using System.Collections;
using UnityEngine;

/// <summary>
/// Insanely basic audio system which supports 3D sound.
/// Ensure you change the 'Sounds' audio source to use 3D spatial blend if you intend to use 3D sounds.
/// </summary>
public class AudioSystem : StaticInstance<AudioSystem> 
{
    [SerializeField] private AudioSource _musicSource;
    [SerializeField] private AudioSource[] _soundsSources;
    [SerializeField] private AudioSource _runningSource;
    [SerializeField] private AudioSource _jumpSource;
    [SerializeField] private AudioSource _corruptedSource;


    private void OnEnable()
    {
        EventManager<bool>.Instance.StartListening("onRunning", PlayRunningEffect);
        EventManager<bool>.Instance.StartListening("onFullyCorrupted", PlayCorruptionffect);
        EventManager<AudioClip>.Instance.StartListening("onPlayClip", PlayClip);
        EventManager<AudioClip>.Instance.StartListening("onPlayJumpClip", PlayJumpClip);
    }

    private void OnDisable()
    {
        EventManager<bool>.Instance.StopListening("onRunning", PlayRunningEffect);
        EventManager<bool>.Instance.StopListening("onFullyCorrupted", PlayCorruptionffect);
        EventManager<AudioClip>.Instance.StopListening("onPlayClip", PlayClip);
        EventManager<AudioClip>.Instance.StopListening("onPlayJumpClip", PlayJumpClip);
    }

    //public void PlayMusic(AudioClip clip)
    //{
    //    _musicSource.clip = clip;
    //    _musicSource.volume = 0.08f;
    //    _musicSource.Play();
    //}

    //public void PlaySound(AudioClip clip, Vector3 pos, float vol = 1)
    //{
    //    _soundsSource.transform.position = pos;
    //    PlaySound(clip, vol);
    //}

    //public void PlaySound(AudioClip clip, float vol = 1)
    //{
    //    _soundsSource.PlayOneShot(clip, vol);
    //}
    
    public void PlayClip(AudioClip clip)
    {
        for (int i = 0; i < _soundsSources.Length; i++)
        {
            if (!_soundsSources[i].isPlaying && GameController.Instance.state == GameState.PLAYING)
            {
                _soundsSources[i].volume = .5f;
                _soundsSources[i].PlayOneShot(clip);
                break;
            }
        }
    }

    public void PlayRunningEffect(bool canPlay)
    {       
        if (canPlay && GameController.Instance.state == GameState.PLAYING)
        {
            if (!_runningSource.isPlaying)
            {
                _runningSource.Play();
            }
        }
        else
        {
            _runningSource.Stop();
        }
    }


    public void PlayJumpClip(AudioClip clip)
    {
        if (!_jumpSource.isPlaying && GameController.Instance.state == GameState.PLAYING)
        {
            _jumpSource.PlayOneShot(clip);
        }
    }

    public void PlayCorruptionffect(bool canPlay)
    {
        if (canPlay && GameController.Instance.state == GameState.PLAYING)
        {
            if (!_corruptedSource.isPlaying)
            {
                _corruptedSource.Play();
            }
        }
        else
        {
            _corruptedSource.Stop();
        }
    }
}