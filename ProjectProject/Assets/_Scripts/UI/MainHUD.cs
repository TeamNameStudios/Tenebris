using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainHUD : MonoBehaviour
{
    public GameObject pauseMenu;
    public List<GameObject> pausePanels;
    public GameObject gameoverPanel;

    private bool pauseState = false;
    public TextMeshProUGUI _pageCount;
    public TextMeshProUGUI _distanceCount;
    public TextMeshProUGUI _timerCount;

    [SerializeField] private TextMeshProUGUI bestDistanceText;
    [SerializeField] private TextMeshProUGUI currentDistanceText;
    [SerializeField] private TextMeshProUGUI currentTimeText;
    private float bestDistance;
    private float currentDistance;
    private TimeSpan currentTime;

    public void SetPauseState(bool state)
    {
        pauseState = !pauseState;

        if (state)
        {
            pausePanels[0].SetActive(false);
            pausePanels[1].SetActive(true);
            pauseMenu.SetActive(true);
        }
        else
        {
            pausePanels[0].SetActive(true);
            pausePanels[1].SetActive(false);
            pauseMenu.SetActive(false);
        }
    }

    public void QuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Application.Quit();
    #endif
    }

    public void UpdatePageCount(int newPageCount)
    {
        _pageCount.text = newPageCount.ToString();
       
    }

    public void UpdateDistanceCount(float newDistanceCount)
    {
        _distanceCount.text = newDistanceCount.ToString();
        currentDistance = newDistanceCount;
    }


    private void Start()
    {
        pausePanels[0].SetActive(true);
        pausePanels[1].SetActive(false);
        pauseMenu.SetActive(false);
        TempGameOver(false);
    }

    private void OnEnable()
    {
        EventManager<bool>.Instance.StartListening("pause", SetPauseState);
        EventManager<int>.Instance.StartListening("UpdatePageCount", UpdatePageCount);
        EventManager<float>.Instance.StartListening("UpdateDistanceCount", UpdateDistanceCount);
        EventManager<bool>.Instance.StartListening("onLevelEnded", TempLevelEnd);
        EventManager<bool>.Instance.StartListening("onGameOver", TempGameOver);
        EventManager<TimeSpan>.Instance.StartListening("onTimer", UpdateTimer);
        EventManager<float>.Instance.StartListening("onBestDistanceUpdated", UpdateBestDistance);
        EventManager<float>.Instance.StartListening("onBestDistanceLoaded", LoadBestDistance);
        EventManager<bool>.Instance.StartListening("onGamePaused", SetPauseState);
    }

    private void OnDisable()
    {
        EventManager<bool>.Instance.StopListening("pause", SetPauseState);
        EventManager<int>.Instance.StopListening("UpdatePageCount", UpdatePageCount);
        EventManager<float>.Instance.StopListening("UpdateDistanceCount", UpdateDistanceCount);
        EventManager<bool>.Instance.StopListening("onLevelEnded", TempLevelEnd);
        EventManager<bool>.Instance.StopListening("onGameOver", TempGameOver);
        EventManager<TimeSpan>.Instance.StopListening("onTimer", UpdateTimer);
        EventManager<float>.Instance.StopListening("onBestDistanceU", UpdateBestDistance); 
        EventManager<float>.Instance.StartListening("onBestDistanceLoaded", LoadBestDistance);
        EventManager<bool>.Instance.StartListening("onGamePaused", SetPauseState);
    }
    public void UpdateTimer(TimeSpan timer)
    {
        _timerCount.text = timer.ToString(@"mm\:ss");
        currentTime = timer;
    }
    public void ReloadScene()
    {
        EventManager<bool>.Instance.TriggerEvent("onSceneReload", true);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void TempLevelEnd(bool state)
    {
        if (state)
        {
        }
    }

    private void TempGameOver(bool state)
    {
        if (state)
        {
            SetGameOverPanel();
            gameoverPanel.SetActive(true);
        }
        else
        {
            gameoverPanel.SetActive(false);
        }
    }

    public void ReloadMainMenu()
    {
        EventManager<bool>.Instance.TriggerEvent("onSceneReload", true);
        SceneManager.LoadScene("MainMenu");
    }

    private void LoadBestDistance(float _bestDistance)
    {
        bestDistance = _bestDistance;
    }

    private void UpdateBestDistance(float _bestDistance)
    {
        bestDistance = _bestDistance;
    }


    public void SetGameOverPanel()
    {
        currentDistanceText.text = "Distance: " + currentDistance.ToString();
        currentTimeText.text = "Time: " + currentTime.ToString(@"mm\:ss");
        bestDistanceText.text = "Best Distance: " + bestDistance.ToString();
    }
}
