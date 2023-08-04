using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class GameController : Singleton<GameController>
{
    private GameState state = GameState.IDLE;
    public GameState State { get => state; private set => state = value; }
    [SerializeField]
    private Player player;

    [SerializeField]
    private Shadow shadow;


    private void Start()
    {
        ChangeState(GameState.STARTING);
    }

    private void OnEnable()
    {
        EventManager<GameState>.Instance.StartListening("onStateChanged", ChangeState);
        EventManager<GameState>.Instance.StartListening("onPlayerDead", ChangeState);
        EventManager<bool>.Instance.StartListening("onMapGenerated", SetGameScene);
        EventManager<bool>.Instance.StartListening("pause", Pause);
    }
    private void OnDisable()
    {
        EventManager<GameState>.Instance.StopListening("onStateChanged", ChangeState);
        EventManager<bool>.Instance.StopListening("onMapGenerated", SetGameScene);
        EventManager<bool>.Instance.StopListening("pause", Pause);
        EventManager<GameState>.Instance.StopListening("onPlayerDead", ChangeState);


    }

    private void Update()
    {
        switch(State)
        {
            case GameState.IDLE:
                break;
            case GameState.STARTING:
                EventManager<bool>.Instance.TriggerEvent("LoadData", true);
                EventManager<bool>.Instance.TriggerEvent("onGameStartingState",true);
                break;
            case GameState.PAUSING:
                Time.timeScale = 0;
                break;
            case GameState.PLAYING:
                Time.timeScale = 1;
                break;
            case GameState.END_LEVEL:
                break;
            case GameState.LOSING:
                EventManager<bool>.Instance.TriggerEvent("onGameOver", true);
                Time.timeScale = 0;
                break;
        }
    }


    private void ChangeState(GameState _state)
    {
        State = _state;
    }


    private void SetGameScene(bool isGameSceneStarted)
    {
        Player _player = Instantiate(player, new Vector2(0, 16), Quaternion.identity).GetComponent<Player>();
        //Shadow _shadow = Instantiate(shadow, new Vector2(-40, 0), Quaternion.identity).GetComponent<Shadow>();
        //_shadow.Setup(_player);
        ChangeState(GameState.PLAYING);
    }


    void Pause (bool isPausing)
    {

        if (isPausing)
        {
            ChangeState(GameState.PAUSING);
        }
        else
        {
            ChangeState(GameState.PLAYING);
        }
    }

}
public enum GameState
{
    IDLE,
    STARTING,
    PAUSING,
    PLAYING,
    END_LEVEL,
    LOSING,
}