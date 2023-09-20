using System;
using UnityEngine;

public class GameController : Singleton<GameController>
{
    public int IsTutorial;
    private GameState state = GameState.IDLE;
    public GameState State { get => state; private set => state = value; }

    #region Reference
    [SerializeField]
    private Player player;
    public Player Player { get { return player; } }
    [SerializeField]
    private Shadow shadow;
    [SerializeField]
    private GameObject tracker;
    [SerializeField]
    private GameObject slowMotionController;
    #endregion

    private float runTime;
    private float timeScale;
    [SerializeField]
    private int pageNumber;
    private int totalPage;

    [SerializeField] private bool trueForSeconds;
    [SerializeField] private float seconds;
    [SerializeField] private float minutes;
    [SerializeField] private float timeScaleIncrement;

    [SerializeField] private bool isSpawnShadow;

    public TimeSpan Timer { get; private set; }
    
    private TimeSpan bestTime;

    private void Start()
    {
        if (IsTutorial == 1)
        {
            Instantiate(slowMotionController);
        }
        ChangeState(GameState.STARTING);
    }

    private void OnEnable()
    {
        EventManager<int>.Instance.StartListening("onTotalPageLoaded", LoadTotalPage);
        EventManager<Collectible>.Instance.StartListening("onCollectiblePickup", AddPage);
        EventManager<GameState>.Instance.StartListening("onStateChanged", ChangeState);
        EventManager<GameState>.Instance.StartListening("onPlayerDead", ChangeState);
        EventManager<bool>.Instance.StartListening("onMapGenerated", SetGameScene);
        EventManager<bool>.Instance.StartListening("onPause", Pause);
        EventManager<string>.Instance.StartListening("onBestTimeLoaded", LoadBestTime);
        EventManager<bool>.Instance.StartListening("onTutorialEnd", TutorialEnd);
        EventManager<int>.Instance.StartListening("onTutorialFlagLoaded", LoadTutorialFlag);

        EventManager<bool>.Instance.TriggerEvent("LoadData", true);
    }
    private void OnDisable()
    {
        EventManager<int>.Instance.StopListening("onTotalPageLoaded", LoadTotalPage);
        EventManager<Collectible>.Instance.StopListening("onCollectiblePickup", AddPage);
        EventManager<GameState>.Instance.StopListening("onStateChanged", ChangeState);
        EventManager<bool>.Instance.StopListening("onMapGenerated", SetGameScene);
        EventManager<bool>.Instance.StopListening("pause", Pause);
        EventManager<GameState>.Instance.StopListening("onPlayerDead", ChangeState);
        EventManager<string>.Instance.StopListening("onBestTimeLoaded", LoadBestTime);
        EventManager<bool>.Instance.StopListening("onTutorialEnd", TutorialEnd);
        EventManager<int>.Instance.StopListening("onTutorialFlagLoaded", LoadTutorialFlag);

    }

    private void Update()
    {
        switch (State)
        {
            case GameState.IDLE:
            case GameState.STARTING:
                runTime = 0;
                timeScale = 1;
                pageNumber = 0;
                EventManager<bool>.Instance.TriggerEvent("onGameStartingState", true);

                break;
            case GameState.PAUSING:
                Time.timeScale = 0;
                break;

            case GameState.PLAYING:

                if (IsTutorial == 0)
                {
                    runTime += Time.unscaledDeltaTime;
                    Timer = TimeSpan.FromSeconds(runTime);
                }

                Time.timeScale = timeScale;


                break;

            case GameState.END_LEVEL:
                break;

            case GameState.LOSING:
                EventManager<int>.Instance.TriggerEvent("SaveTotalPage", totalPage + pageNumber);
                if (Timer > bestTime)
                {
                    bestTime = Timer;
                    SaveBestTime(bestTime);
                }

                EventManager<bool>.Instance.TriggerEvent("onGameOver", true);
                EventManager<bool>.Instance.TriggerEvent("onReset", true);
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
        Instantiate(player, new Vector2(0, 15), Quaternion.identity).GetComponent<Player>();
        if (IsTutorial == 0)
        {
            EventManager<SoundEnum>.Instance.TriggerEvent("onPlayMusic", SoundEnum.gameMusic);
            SpawnShadow();
        }
        Instantiate(tracker);
        EventManager<bool>.Instance.TriggerEvent("LoadData", true);
        ChangeState(GameState.PLAYING);
    }

    private void TutorialEnd(bool value)
    {
        SpawnShadow();
        IsTutorial = 0;
        EventManager<int>.Instance.TriggerEvent("SaveTutorialFlag", IsTutorial);
        EventManager<SoundEnum>.Instance.TriggerEvent("onPlayMusic", SoundEnum.gameMusic);
    }

    private void SpawnShadow()
    {
        if (isSpawnShadow)
        {
            Shadow _shadow = Instantiate(shadow, new Vector2(-35, 0), Quaternion.identity).GetComponent<Shadow>();
            _shadow.Setup(player);
            EventManager<Shadow>.Instance.TriggerEvent("onSetShadow", _shadow);
        }
    }

    public void Pause(bool _isPausing)
    {
        GameState gameState = state == GameState.PLAYING ? GameState.PAUSING : state == GameState.PAUSING ? GameState.PLAYING : GameState.PAUSING;
        bool isPausing = gameState != GameState.PLAYING;
        ChangeState(gameState);
        if (isPausing)
        {
            EventManager<bool>.Instance.TriggerEvent("onPauseAll", isPausing);
        }
        else
        {
            EventManager<bool>.Instance.TriggerEvent("onUnPauseAll", isPausing);
        }

        EventManager<bool>.Instance.TriggerEvent("onGamePaused", isPausing);
    }

    private void AddPage(Collectible collectible)
    {
        pageNumber += collectible.PageCount;
        EventManager<int>.Instance.TriggerEvent("UpdatePageCount", pageNumber);//UI
    }

    private void LoadTotalPage(int _totalPage)
    {
        totalPage = _totalPage;
    }

    private void SaveBestTime(TimeSpan bestTime)
    {
        TimeSpan newTimeSpan = new TimeSpan(bestTime.Hours, bestTime.Minutes, bestTime.Seconds);

        string timeString = newTimeSpan.ToString();
        EventManager<string>.Instance.TriggerEvent("SaveBestTime", timeString);
    }

    private void LoadBestTime(string timeString)
    {
        TimeSpan timeSpan = TimeSpan.Parse(timeString);
        bestTime = timeSpan;
    }

    private void LoadTutorialFlag(int tutorialFlag)
    {
        IsTutorial = tutorialFlag;
    }

    //#region GAME-RUN MANAGER
    //[Header("GAME-RUN MANAGER")]
    //private bool hasIncremented = true;

    //private void ManageRun(TimeSpan timeSpan)
    //{
    //    if (trueForSeconds)
    //    {
    //        ManageRunBySeconds(timeSpan);
    //    }
    //    else if (!trueForSeconds)
    //    {
    //        ManageRunByMinutes(timeSpan);
    //    }
    //}


    //#endregion
}
public enum GameState
{
    IDLE,
    TUTORIAL,
    STARTING,
    PAUSING,
    PLAYING,
    END_LEVEL,
    LOSING,
}