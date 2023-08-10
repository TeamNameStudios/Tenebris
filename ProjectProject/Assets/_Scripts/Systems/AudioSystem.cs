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

    private void OnEnable()
    {
        EventManager<bool>.Instance.StartListening("onRunning", PlayRunningEffect);
        EventManager<AudioClip>.Instance.StartListening("onPlayClip", PlayClip);
        EventManager<AudioClip>.Instance.StartListening("onPlayJumpClip", PlayJumpClip);
    }

    private void OnDisable()
    {
        EventManager<bool>.Instance.StopListening("onRunning", PlayRunningEffect);
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

    public void PlayRunningEffect(bool canPlay)
    {       
        if (canPlay)
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

    public void PlayClip(AudioClip clip)
    {
        for (int i = 0; i < _soundsSources.Length; i++)
        {
            if (!_soundsSources[i].isPlaying)
            {
                _soundsSources[i].volume = .5f;
                _soundsSources[i].PlayOneShot(clip);
                break;
            }
        }
    }

    public void PlayJumpClip(AudioClip clip)
    {
        if (!_jumpSource.isPlaying)
        {
            _jumpSource.PlayOneShot(clip);
        }
    }
}