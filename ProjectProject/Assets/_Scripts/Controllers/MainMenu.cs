using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public List<GameObject> panels;
    private Dictionary<string, GameObject> _panels = new Dictionary<string, GameObject>();
    public string activePanelName;

    private void Start()
    {
        panels.ForEach(p => 
        {
            p.SetActive(false);
            _panels.Add(p.name, p);
        });
        EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", 15f);
        _panels[activePanelName].SetActive(true);
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
    public void ShowCredits()
    {

    }

    public void ChangeActivePanel(string panelName)
    {
        _panels[activePanelName].SetActive(false);
        _panels[panelName].SetActive(true);
        activePanelName = panelName;
    }

}
