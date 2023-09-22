using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputController : Singleton<InputController>
{
    public KeyCode jumpKey;
    public KeyCode dashKey;
    public KeyCode grappleKey;

    protected override void Awake()
    {
        base.Awake();

        jumpKey = ResourceSystem.Instance.jumpKey;
        dashKey = ResourceSystem.Instance.dashKey;
        grappleKey = ResourceSystem.Instance.grappleKey;
    }

    private void Update()
    {
        if (GameController.Instance.State == GameState.PLAYING && GameController.Instance != null)
        {
            float directionX = Input.GetAxisRaw("Horizontal");
            float directionY = Input.GetAxisRaw("Vertical");
            EventManager<Vector2>.Instance.TriggerEvent("movement", new Vector2(directionX, directionY));

            if (Input.GetKeyDown(jumpKey))
            {
                EventManager<bool>.Instance.TriggerEvent("jumpMovement", true);
            }
            if (Input.GetKeyDown(dashKey))
            {
                EventManager<bool>.Instance.TriggerEvent("isDashing", true);
            }
            if (Input.GetKeyDown(grappleKey))
            {
                EventManager<bool>.Instance.TriggerEvent("isGrappling", true);
            }
        }

        if (Input.GetKeyUp(KeyCode.Escape) && GameController.Instance.State != GameState.TUTORIAL)
        {
            EventManager<bool>.Instance.TriggerEvent("onPause", true);
        }
    }
}

public enum ActionKeys
{
    JUMP,
    DASH,
    GRAPPLE,
    PAUSE,
    LEFT_1,
    LEFT_2,
    RIGHT_1,
    RIGHT_2,
}
