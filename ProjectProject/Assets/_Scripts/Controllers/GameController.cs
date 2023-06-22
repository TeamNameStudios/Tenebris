using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    public static event Action<GameState> OnBeforeStateChanged;
    public static event Action<GameState> OnAfterStateChanged;
    [SerializeField]
    private CultistType cultistType;
    public GameState State { get; private set; }

    void Start() => ChangeState(GameState.Idle);

    public void StartGame()
    {
        //AudioSystem.Instance.PlayMusic();
        ChangeState(GameState.Starting);
    }
    
    public void ChangeState(GameState newState)
    {
        OnBeforeStateChanged?.Invoke(newState);

        State = newState;
        switch (newState)
        {
            case GameState.Idle:
                HandleIdle();
                break;
            case GameState.Starting:
                HandleStarting();
                break;
            case GameState.SpawningPlayer:
                HandleSpawningCultist();
                break;
            case GameState.Lose:
                HandleLose();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnAfterStateChanged?.Invoke(newState);

        Debug.Log($"New state: {newState}");
    }

    private void HandleIdle() {
        Time.timeScale = 0;
    }

    private void HandleStarting()
    {
        ChangeState(GameState.SpawningPlayer);
    }

    private void HandleSpawningCultist()
    {
        CultistController.Instance.SpawnCultist(cultistType);
    }
               
    private void HandlePlaying()
    {
        Time.timeScale = 1;         
    }

    private void HandleLose()
    {
        Time.timeScale = 0;
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
}
public enum GameState
{
    Idle,
    Starting,
    SpawningPlayer,
    Lose,
}