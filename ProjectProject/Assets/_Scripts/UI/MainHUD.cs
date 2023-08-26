using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MainHUD : MonoBehaviour
{
    public GameObject pageCount;
    public GameObject distanceCount;
    public GameObject pauseMenu;
    public List<GameObject> pausePanels;
    public GameObject endLevelPanel;
    public GameObject gameoverPanel;

    private bool pauseState = false;
    private TextMeshProUGUI _pageCount;
    private TextMeshProUGUI _distanceCount;

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
    }

    private void Start()
    {
        _pageCount = pageCount.GetComponent<TextMeshProUGUI>();
        _distanceCount = distanceCount.GetComponent<TextMeshProUGUI>();
        pausePanels[0].SetActive(true);
        pausePanels[1].SetActive(false);
        pauseMenu.SetActive(false);
        endLevelPanel.SetActive(false);
        TempGameOver(false);
    }

    private void OnEnable()
    {
        EventManager<bool>.Instance.StartListening("pause", SetPauseState);
        EventManager<int>.Instance.StartListening("UpdatePageCount", UpdatePageCount);
        EventManager<float>.Instance.StartListening("UpdateDistanceCount", UpdateDistanceCount);
        EventManager<bool>.Instance.StartListening("onLevelEnded", TempLevelEnd);
        EventManager<bool>.Instance.StartListening("onGameOver", TempGameOver);
    }

    private void OnDisable()
    {
        EventManager<bool>.Instance.StopListening("pause", SetPauseState);
        EventManager<int>.Instance.StopListening("UpdatePageCount", UpdatePageCount);
        EventManager<float>.Instance.StopListening("UpdateDistanceCount", UpdateDistanceCount);
        EventManager<bool>.Instance.StopListening("onLevelEnded", TempLevelEnd);
        EventManager<bool>.Instance.StopListening("onGameOver", TempGameOver);
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
            endLevelPanel.SetActive(state);
        }
    }

    private void TempGameOver(bool state)
    {
        if (state)
        {
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
}
