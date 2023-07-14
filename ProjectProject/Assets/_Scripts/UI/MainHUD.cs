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

    private bool pauseState = false;
    private TextMeshProUGUI _pageCount;
    private TextMeshProUGUI _distanceCount;

    public void SetPauseState(bool state)
    {
        //pauseState = !pauseState;

        if (state)
        {
            pausePanels[0].SetActive(false);
            pausePanels[1].SetActive(true);
            pauseMenu.SetActive(true);
            //Time.timeScale = 0f;
        }
        else
        {
            pausePanels[0].SetActive(true);
            pausePanels[1].SetActive(false);
            pauseMenu.SetActive(false);
            //Time.timeScale = 1f;
        }
    }

    public void QuitGame()
    {
    #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
    #else
        Apllication.Quit();
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
    }

    private void OnEnable()
    {
        EventManager<bool>.Instance.StartListening("pause", SetPauseState);
        EventManager<int>.Instance.StartListening("UpdatePageCount", UpdatePageCount);
        EventManager<float>.Instance.StartListening("UpdateDistanceCount", UpdateDistanceCount);
        EventManager<bool>.Instance.StartListening("onLevelEnded", TempLevelEnd);
    }

    private void OnDisable()
    {
        EventManager<bool>.Instance.StopListening("pause", SetPauseState);
        EventManager<int>.Instance.StopListening("UpdatePageCount", UpdatePageCount);
        EventManager<float>.Instance.StopListening("UpdateDistanceCount", UpdateDistanceCount);
        EventManager<bool>.Instance.StopListening("onLevelEnded", TempLevelEnd);
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void TempLevelEnd(bool state)
    {
        if (state)
        {
            endLevelPanel.SetActive(state);
        }
    }
}
