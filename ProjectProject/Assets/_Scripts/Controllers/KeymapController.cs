using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeymapController : Singleton<KeymapController>
{
    public KeyCode jumpKey;
    public KeyCode dashKey;
    public KeyCode grappleKey;

    public Dictionary<ActionKeys, KeyCode> keyDict = new Dictionary<ActionKeys, KeyCode>();

    public bool canRebind = true;

    private void OnEnable()
    {
        EventManager<int>.Instance.StartListening("onJumpKeyLoaded", LoadJumpKey); 
        EventManager<int>.Instance.StartListening("onDashKeyLoaded", LoadDashKey); 
        EventManager<int>.Instance.StartListening("onGrappleKeyLoaded", LoadGrappleKey);
    }

    private void OnDisable()
    {
        EventManager<int>.Instance.StopListening("onJumpKeyLoaded", LoadJumpKey);
        EventManager<int>.Instance.StopListening("onDashKeyLoaded", LoadDashKey);
        EventManager<int>.Instance.StopListening("onGrappleKeyLoaded", LoadGrappleKey);
    }

    private void Start()
    {
        EventManager<bool>.Instance.TriggerEvent("LoadControls", true);

        keyDict.Add(ActionKeys.JUMP, jumpKey);
        keyDict.Add(ActionKeys.JUMP_1, KeyCode.W);
        keyDict.Add(ActionKeys.JUMP_2, KeyCode.UpArrow);
        keyDict.Add(ActionKeys.DASH, dashKey);
        keyDict.Add(ActionKeys.GRAPPLE, grappleKey);
        keyDict.Add(ActionKeys.PAUSE, KeyCode.Escape);
        keyDict.Add(ActionKeys.LEFT_1, KeyCode.A);
        keyDict.Add(ActionKeys.LEFT_2, KeyCode.LeftArrow);
        keyDict.Add(ActionKeys.RIGHT_1, KeyCode.D);
        keyDict.Add(ActionKeys.RIGHT_2, KeyCode.RightArrow);
    }

    public void StartRebind(ActionKeys keys)
    {
        canRebind = true;
        StartCoroutine(RebindCO(keys));
    }

    public IEnumerator RebindCO(ActionKeys key)
    {
        while (canRebind)
        {
            foreach (KeyCode keyCode in System.Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(keyCode))
                {
                    if (keyDict.ContainsValue(keyCode))
                    {
                        canRebind = false;
                        EventManager<bool>.Instance.TriggerEvent("onRebindFailed", true);
                    }
                    else
                    {
                        keyDict[key] = keyCode;
                        SaveKeys();
                        canRebind = false;
                    }
                }
            }
            yield return null;

        }
    }

    private void SaveKeys()
    {
        jumpKey = keyDict[ActionKeys.JUMP];
        dashKey = keyDict[ActionKeys.DASH];
        grappleKey = keyDict[ActionKeys.GRAPPLE];
        EventManager<int>.Instance.TriggerEvent("SaveJumpKey", (int)jumpKey);
        EventManager<int>.Instance.TriggerEvent("SaveDashKey", (int)dashKey);
        EventManager<int>.Instance.TriggerEvent("SaveGrappleKey", (int)grappleKey);

        EventManager<bool>.Instance.TriggerEvent("onRebindCompleted", true);
    }

    public void ResetKeys()
    {
        keyDict[ActionKeys.JUMP] = KeyCode.RightShift;
        keyDict[ActionKeys.DASH] = KeyCode.Space;
        keyDict[ActionKeys.GRAPPLE] = KeyCode.LeftShift;
        SaveKeys();
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
