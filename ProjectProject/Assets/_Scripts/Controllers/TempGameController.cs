using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempGameController : Singleton<TempGameController>
{
    private int pageNumber;

    private GameState state = GameState.PLAYING;
    public GameState State { get => state; private set => state = value; }

    private void OnEnable()
    {
        EventManager<int>.Instance.StartListening("onCollectiblePickup", AddPage);
        EventManager<bool>.Instance.StartListening("pause", Pause);
        EventManager<GameState>.Instance.StartListening("onPlayerDead", ChangeState);
    }

    private void AddPage(int number)
    {
        pageNumber += number;
        EventManager<int>.Instance.TriggerEvent("UpdatePageCount", pageNumber);
    }

    private void Update()
    {
        switch (State)
        {
            case GameState.IDLE:
                break;
            case GameState.STARTING:
                break;
            case GameState.PAUSING:
                Time.timeScale = 0;
                break;
            case GameState.PLAYING:
                Time.timeScale = 1;

                if(Input.GetKeyDown(KeyCode.P))
                {
                    ResourceSystem.Instance.ChangeBaseProbability(LevelDifficulty.EASY, 50);
                }

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

    void Pause(bool isPausing)
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
