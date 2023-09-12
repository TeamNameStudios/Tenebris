using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotionController : Singleton<SlowMotionController>
{
    public float t;
    [SerializeField] private float slowMoSpeed;


    protected override void Awake()
    {
        base.Awake();
        EventManager<bool>.Instance.TriggerEvent("onTutorialStarted", true);
    }

    private void Update()
    {
        switch (GameController.Instance.State)
        {
            case GameState.IDLE:
                break;
            case GameState.TUTORIAL:
                Time.timeScale = Mathf.Lerp(1, 0, t);
                t += Time.unscaledDeltaTime * slowMoSpeed;
                break;
            case GameState.PLAYING:
                break;
        }
    }
}