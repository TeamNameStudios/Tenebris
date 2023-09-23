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
    }

    private void OnEnable()
    {
        EventManager<int>.Instance.StartListening("onJumpKeyLoaded", LoadJumpKey);
        EventManager<int>.Instance.StartListening("onDashKeyLoaded", LoadDashKey);
        EventManager<int>.Instance.StartListening("onGrappleKeyLoaded", LoadGrappleKey);

        EventManager<bool>.Instance.TriggerEvent("LoadControls", true);
    }

    private void OnDisable()
    {
        EventManager<int>.Instance.StopListening("onJumpKeyLoaded", LoadJumpKey);
        EventManager<int>.Instance.StopListening("onDashKeyLoaded", LoadDashKey);
        EventManager<int>.Instance.StopListening("onGrappleKeyLoaded", LoadGrappleKey);
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

    private void LoadJumpKey(int _jumpKey)
    {
        jumpKey = (KeyCode)System.Enum.ToObject(typeof(KeyCode), _jumpKey);
    }

    private void LoadDashKey(int _dashKey)
    {
        dashKey = (KeyCode)System.Enum.ToObject(typeof(KeyCode), _dashKey);
    }

    private void LoadGrappleKey(int _grappleKey)
    {
        grappleKey = (KeyCode)System.Enum.ToObject(typeof(KeyCode), _grappleKey);
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
