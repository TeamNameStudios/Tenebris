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
            Time.timeScale = 0f;
        }
        else
        {
            pausePanels[0].SetActive(true);
            pausePanels[1].SetActive(false);
            Time.timeScale = 1f;
        }
    }

    public void UpdatePageCount(int newPageCount)
    {
        _pageCount.text = newPageCount.ToString();
    }

    public void UpdateDistanceCount(int newDistanceCount)
    {
        _distanceCount.text = newDistanceCount.ToString();
    }

    // Start is called before the first frame update
    void Start()
    {
        _pageCount = pageCount.GetComponent<TextMeshPro>();
        _distanceCount = distanceCount.GetComponent<TextMeshPro>();
    }
}
