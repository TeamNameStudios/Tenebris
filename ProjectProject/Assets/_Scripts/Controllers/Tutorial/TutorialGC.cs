using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialGC : Singleton<TutorialGC>
{
    public bool welcomeTutorialPlayed = false;
    public float t;
    [SerializeField] private float slowMoSpeed;


    protected override void Awake()
    {
        base.Awake();
        EventManager<bool>.Instance.TriggerEvent("onTutorialStarted", true);
    }

    private void Update()
    {
        switch (GameController.Instance.state)
        {
            case GameState.IDLE:

                Time.timeScale = Mathf.Lerp(0, 1, t);
                t += Time.unscaledDeltaTime * slowMoSpeed;

                if (t >= 1)
                {
                    EventManager<GameState>.Instance.TriggerEvent("onStateChanged", GameState.PLAYING);
                }

                break;

            case GameState.TUTORIAL:

                Time.timeScale = Mathf.Lerp(1, 0, t);
                t += Time.unscaledDeltaTime * slowMoSpeed;

                if (!welcomeTutorialPlayed)
                {
                    EventManager<string>.Instance.TriggerEvent("onPlayDialogue", "welcome");
                    welcomeTutorialPlayed = true;
                }

                break;

            case GameState.PLAYING:

                break;
        }

        if (Input.GetKeyDown(KeyCode.KeypadPlus))
        {
            //canSlow = true;
            GameController.Instance.state = GameState.TUTORIAL;
        }
    }
}