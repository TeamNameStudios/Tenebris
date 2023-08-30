using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public string activePanelName;
    public float fadeTimer;
    public AnimationCurve fadeOut;
    public List<GameObject> panels;

    [SerializeField] private TextMeshProUGUI bestDistanceText;
    [SerializeField] private TextMeshProUGUI bestTimeText;

    [SerializeField] private float bestDistance;
    [SerializeField] private string bestTime;

    #region Animation
    private bool _transitionNewScene;
    private TransitionState _transitionNewPanel = 0;
    private GameObject _panelToTransition;
    private GameObject _panelFromTransition;
    private Dictionary<string, GameObject> _panels = new Dictionary<string, GameObject>();
    private float _activeTimer = 0;

    private enum TransitionState
    {
        None,
        FadeOut,
        FadeIn
    }
    #endregion

    private void Awake()
    {
        
    }

    private void OnEnable()
    {
        Time.timeScale = 1;
        // as right now, every panel is disabled at start, this way there won't be any trouble on setting the right visibility while developing
        panels.ForEach(p =>
        {
            p.SetActive(false);
            _panels.Add(p.name, p);
        });
        EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", 15f);
        _panels[activePanelName].SetActive(true);
        EventManager<float>.Instance.StartListening("onBestDistanceLoaded", LoadBestDistance);
        EventManager<string>.Instance.StartListening("onBestTimeLoaded", LoadBestTime);
        EventManager<bool>.Instance.TriggerEvent("LoadData", true);
    }
    private void OnDisable()
    {
        EventManager<string>.Instance.StopListening("onBestTimeLoaded", LoadBestTime);
        EventManager<float>.Instance.StopListening("onBestDistanceLoaded", LoadBestDistance);
    }

    /// <summary>
    /// Event called once the PLAY button is Clicked or Tapped, it set up the animation for the Fade in Black
    /// </summary>
    public void PlayGame()
    {
        _transitionNewScene = true;
        _activeTimer = 0;
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    public void ShowCredits()
    {

    }

    private void LoadBestDistance(float _bestDistance)
    {
        bestDistance = _bestDistance;
        bestDistanceText.text = bestDistance + " m";
    }

    private void LoadBestTime(string timeString)
    {
        bestTime = timeString;
        bestTimeText.text = bestTime;
    }


    /// <summary>
    /// Event for changing the Active Panel in the Main Menu, even if brief, it starts a brief animation of FadeIn and FadeOut
    /// </summary>
    /// <param name="panelName"></param>
    public void ChangeActivePanel(string panelName)
    {
        _panelFromTransition = _panels[activePanelName];
        _panelToTransition = _panels[panelName];
        _activeTimer = fadeTimer / 2;
        _transitionNewPanel++;
    }

    /// <summary>
    /// The Fixed Update is used mainly to control the animation of the GUI
    /// </summary>
    private void FixedUpdate()
    {
        if (_transitionNewScene)
        {
            GameObject fadePanel = _panels["FadePanel"];
            fadePanel.SetActive(true);
            Image fadeImage = fadePanel.GetComponent<Image>();

            if (_activeTimer < fadeTimer)
            {
                _activeTimer += Time.fixedDeltaTime;
                Color fadeCol = fadeImage.color;
                fadeCol.a =  fadeOut.Evaluate(_activeTimer);
                fadeImage.color = fadeCol;
            }
            else
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
                _transitionNewScene = false;
            }
        }
        switch(_transitionNewPanel)
        {
            // Current Panel fades out
            case TransitionState.FadeOut:
                _activeTimer -= Time.fixedDeltaTime * 2;
                if (_activeTimer > 0)
                    _panelFromTransition.GetComponent<CanvasGroup>().alpha = fadeOut.Evaluate(_activeTimer);
                else
                    _transitionNewPanel++;
                break;
            // New Panel fades in
            case TransitionState.FadeIn:
                if(_activeTimer <= 0)
                {
                    _panelFromTransition.SetActive(false);
                    _panelToTransition.SetActive(true);
                }
                _activeTimer += Time.fixedDeltaTime * 2;
                if (_activeTimer < fadeTimer / 2)
                    _panelToTransition.GetComponent<CanvasGroup>().alpha = fadeOut.Evaluate(_activeTimer);
                else
                {
                    // re-sets all the private variable for a new transition
                    _transitionNewPanel = 0;
                    activePanelName = _panelToTransition.name;
                }   
                break;

            default:
                break;
                
        }
    }

}
