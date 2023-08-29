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
    [SerializeField] private AudioSource _playerDead;
    [SerializeField] private AudioClip DamageClip;



    private void OnEnable()
    {
        EventManager<bool>.Instance.StartListening("onRunning", PlayRunningEffect);
        EventManager<bool>.Instance.StartListening("onFullyCorrupted", PlayCorruptionffect);
        EventManager<AudioClip>.Instance.StartListening("onPlayClip", PlayClip);
        EventManager<AudioClip>.Instance.StartListening("onPlayJumpClip", PlayJumpClip);
        EventManager<bool>.Instance.StartListening("onGameOver", PlayPlayerDead);
        EventManager<bool>.Instance.StartListening("onHit", PlayHitClip);
    }

    private void OnDisable()
    {
        EventManager<bool>.Instance.StopListening("onRunning", PlayRunningEffect);
        EventManager<bool>.Instance.StopListening("onFullyCorrupted", PlayCorruptionffect);
        EventManager<AudioClip>.Instance.StopListening("onPlayClip", PlayClip);
        EventManager<AudioClip>.Instance.StopListening("onPlayJumpClip", PlayJumpClip);
        EventManager<bool>.Instance.StopListening("onGameOver", PlayPlayerDead);
        EventManager<bool>.Instance.StopListening("onHit", PlayHitClip);

    }

    private void Update()
    {
        if (GameController.Instance != null)
        {
            if (GameController.Instance.state == GameState.PAUSING)
            {
                _musicSource.Pause();
            }
            else if (GameController.Instance.state == GameState.PLAYING)
            {
                if (!_musicSource.isPlaying)
                {
                    PlayMusic(_musicSource.clip);
                }
                
                _musicSource.UnPause();
            }
            else if (GameController.Instance.state == GameState.LOSING)
            {
                _musicSource.Stop();
            }
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        _musicSource.clip = clip;
        _musicSource.volume = 0.2f;
        _musicSource.Play();
    }

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
    
    public void PlayHitClip(bool value)
    {
        for (int i = 0; i < _soundsSources.Length; i++)
        {
            if (!_soundsSources[i].isPlaying && GameController.Instance.state == GameState.PLAYING)
            {
                _soundsSources[i].PlayOneShot(DamageClip);
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

    public void PlayPlayerDead(bool canPlay)
    {
        if (!_playerDead.isPlaying)
        {
            _playerDead.PlayOneShot(_playerDead.clip);
        }
    }
}