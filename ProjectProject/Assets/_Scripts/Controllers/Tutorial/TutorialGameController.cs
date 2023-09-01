using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGameController : Singleton<TutorialGameController>
{
    public GameState state = GameState.IDLE;
    public GameState State { get => state; private set => state = value; }
    [SerializeField]
    private Player player;

    [SerializeField]
    private Shadow shadow;

    [SerializeField] GameObject tracker;

    private float runTime;
    [SerializeField] private float timeScale;
    [SerializeField]
    private int pageNumber;

    private TimeSpan time;

    private bool canSlow = false;
    private float t;
    [SerializeField] private float slowMoSpeed;


    private void Start()
    {
        ChangeState(GameState.STARTING);
    }

    private void OnEnable()
    {
        EventManager<int>.Instance.StartListening("onCollectiblePickup", AddPage);
        EventManager<GameState>.Instance.StartListening("onStateChanged", ChangeState);
        EventManager<GameState>.Instance.StartListening("onPlayerDead", ChangeState);
        EventManager<bool>.Instance.StartListening("pause", Pause);
    }

    private void OnDisable()
    {
        EventManager<int>.Instance.StopListening("onCollectiblePickup", AddPage);
        EventManager<GameState>.Instance.StopListening("onStateChanged", ChangeState);
        EventManager<GameState>.Instance.StopListening("onPlayerDead", ChangeState);
        EventManager<bool>.Instance.StopListening("pause", Pause);
    }

    private void Update()
    {
        switch (State)
        {
            case GameState.IDLE:

                Time.timeScale = Mathf.Lerp(0, 1, t);
                t += Time.unscaledDeltaTime * slowMoSpeed;

                if (t >= 1)
                {
                    state = GameState.PLAYING;
                }

                break;

            case GameState.STARTING:
                runTime = 0;
                timeScale = 1;
                pageNumber = 0;
                SetGameScene(true);
                break;
            case GameState.PAUSING:
                Time.timeScale = 0;
                break;

            case GameState.TUTORIAL:

                Time.timeScale = Mathf.Lerp(1, 0, t);
                t += Time.unscaledDeltaTime * slowMoSpeed;



                break;
            
            case GameState.PLAYING:

                
                runTime += Time.unscaledDeltaTime;
                time = TimeSpan.FromSeconds(runTime);
                EventManager<TimeSpan>.Instance.TriggerEvent("onTimer", time);



                break;

            case GameState.END_LEVEL:
                break;

            case GameState.LOSING:

                EventManager<bool>.Instance.TriggerEvent("onGameOver", true);
                Time.timeScale = 0;
                //state = GameState.END_LEVEL;
                break;
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            //canSlow = true;
            state = GameState.TUTORIAL;
        }
    }

    private void ChangeState(GameState _state)
    {
        State = _state;

        if (_state == GameState.IDLE)
        {
            t = 0;
        }
    }

    private void SetGameScene(bool isGameSceneStarted)
    {
        Instantiate(player, new Vector2(0, 30), Quaternion.identity).GetComponent<Player>();

        //Instantiate(tracker, new Vector3(0, 0, 0), Quaternion.identity);
        ChangeState(GameState.PLAYING);
    }

    public void Pause(bool isPausing)
    {
        if (isPausing)
        {
            ChangeState(GameState.PAUSING);
            EventManager<bool>.Instance.TriggerEvent("onGamePaused", isPausing);
        }
        else
        {
            EventManager<bool>.Instance.TriggerEvent("onGamePaused", isPausing);
            Time.timeScale = timeScale;
            ChangeState(GameState.PLAYING);
        }
    }

    private void AddPage(int number)
    {
        pageNumber += number;
        EventManager<int>.Instance.TriggerEvent("UpdatePageCount", pageNumber);//UI
    }
}
