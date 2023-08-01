using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TempGameController : Singleton<TempGameController>
{
    private int pageNumber;

    private GameState state;
    public GameState State { get => state; private set => state = value; }

    private void Start()
    {
        state = GameState.IDLE;
    }

    private void OnEnable()
    {
        EventManager<int>.Instance.StartListening("onCollectiblePickup", AddPage);
        EventManager<bool>.Instance.StartListening("pause", Pause);
        EventManager<GameState>.Instance.StartListening("onPlayerDead", ChangeState);
        EventManager<int>.Instance.StartListening("onPageLoaded", LoadNumberPage);
        EventManager<List<PowerUp>>.Instance.StartListening("onPowerUpLoaded", LoadPowerUp);

    }

    private void OnDisable()
    {
        EventManager<int>.Instance.StopListening("onCollectiblePickup", AddPage);
        EventManager<bool>.Instance.StopListening("pause", Pause);
        EventManager<GameState>.Instance.StopListening("onPlayerDead", ChangeState);
        EventManager<int>.Instance.StopListening("onPageLoaded", LoadNumberPage);
        EventManager<List<PowerUp>>.Instance.StopListening("onPowerUpLoaded", LoadPowerUp);

    }

    private void AddPage(int number)
    {
        pageNumber += number;
        EventManager<int>.Instance.TriggerEvent("SavePage", pageNumber);
        EventManager<int>.Instance.TriggerEvent("UpdatePageCount", pageNumber);
    }

    private void Update()
    {
        switch (State)
        {
            case GameState.IDLE:
                EventManager<bool>.Instance.TriggerEvent("LoadData", true);
                ChangeState(GameState.PLAYING);
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

    private void LoadNumberPage(int numberOfPages)
    {
        pageNumber = numberOfPages;
        EventManager<int>.Instance.TriggerEvent("UpdatePageCount", pageNumber);

    }

    private void LoadPowerUp(List<PowerUp> powerUpList) {
        //pageNumber += number;
        //EventManager<int>.Instance.TriggerEvent("SavePage", pageNumber);
        //EventManager<int>.Instance.TriggerEvent("UpdatePageCount", pageNumber);
    }

}
