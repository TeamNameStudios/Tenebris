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
        EventManager<string>.Instance.StartListening("onJumpKeyLoaded", LoadJumpKey); 
        EventManager<string>.Instance.StartListening("onDashKeyLoaded", LoadDashKey); 
        EventManager<string>.Instance.StartListening("onGrappleKeyLoaded", LoadGrappleKey);

        Debug.Log("KEYMAP CONTROLLER ENABLED");

    }

    private void OnDisable()
    {
        EventManager<string>.Instance.StopListening("onJumpKeyLoaded", LoadJumpKey);
        EventManager<string>.Instance.StopListening("onDashKeyLoaded", LoadDashKey);
        EventManager<string>.Instance.StopListening("onGrappleKeyLoaded", LoadGrappleKey);
    }

    private void Start()
    {
        EventManager<bool>.Instance.TriggerEvent("LoadControls", true);

        keyDict.Add(ActionKeys.JUMP, jumpKey);
        keyDict.Add(ActionKeys.DASH, dashKey);
        keyDict.Add(ActionKeys.GRAPPLE, grappleKey);
        keyDict.Add(ActionKeys.PAUSE, KeyCode.Escape);
        keyDict.Add(ActionKeys.LEFT_1, KeyCode.A);
        keyDict.Add(ActionKeys.LEFT_2, KeyCode.LeftArrow);
        keyDict.Add(ActionKeys.RIGHT_1, KeyCode.D);
        keyDict.Add(ActionKeys.RIGHT_2, KeyCode.RightArrow);

        ResourceSystem.Instance.ShareControlsKeys(jumpKey, dashKey, grappleKey);
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
        EventManager<string>.Instance.TriggerEvent("SaveJumpKey", jumpKey.ToString());
        EventManager<string>.Instance.TriggerEvent("SaveDashKey", dashKey.ToString());
        EventManager<string>.Instance.TriggerEvent("SaveGrappleKey", grappleKey.ToString());

        ResourceSystem.Instance.ShareControlsKeys(jumpKey, dashKey, grappleKey);
    }

    private void LoadJumpKey(string _jumpKey)
    {
        jumpKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), _jumpKey);
    }

    private void LoadDashKey(string _dashKey)
    {
        dashKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), _dashKey);
    }

    private void LoadGrappleKey(string _grappleKey)
    {
        grappleKey = (KeyCode)System.Enum.Parse(typeof(KeyCode), _grappleKey);
    }
}
