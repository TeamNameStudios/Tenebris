using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MainHUD : MonoBehaviour
{
    public GameObject pageCount;
    public GameObject distanceCount;
    public GameObject pauseMenu;
    public List<GameObject> pausePanels;

    private bool pauseState = false;
    private TextMeshPro _pageCount;
    private TextMeshPro _distanceCount;

    public void SetPauseState()
    {
        pauseState = !pauseState;

        if (pauseState)
        {
            pausePanels[0].SetActive(false);
            pausePanels[1].SetActive(true);
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            pausePanels[0].SetActive(true);
            pausePanels[1].SetActive(false);
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
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

    public void UpdateDistanceCount(int newDistanceCount)
    {
        _distanceCount.text = newDistanceCount.ToString();
    }

    private void OnEnable()
    {
        _pageCount = pageCount.GetComponent<TextMeshPro>();
        _distanceCount = distanceCount.GetComponent<TextMeshPro>();
        pausePanels[0].SetActive(true);
        pausePanels[1].SetActive(false);
        pauseMenu.SetActive(false);

        EventManager<int>.Instance.StartListening("UpdatePauseState", SetPauseState);
        EventManager<int>.Instance.StartListening("UpdatePageCount", UpdatePageCount);
        EventManager<int>.Instance.StartListening("UpdateDistanceCount", UpdateDistanceCount);
    }

    private void OnDisable()
    {
        EventManager<int>.Instance.StopListening("UpdatePauseState", SetPauseState);
        EventManager<int>.Instance.StopListening("UpdatePageCount", UpdatePageCount);
        EventManager<int>.Instance.StopListening("UpdateDistanceCount", UpdateDistanceCount);
    }


}
