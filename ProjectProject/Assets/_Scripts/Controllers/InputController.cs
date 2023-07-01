using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : Singleton<InputController>
{

    private void Update()
    {

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            EventManager<Vector2>.Instance.TriggerEvent("leftMovement", Vector2.left);
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
        {
            EventManager<Vector2>.Instance.TriggerEvent("leftMovement", Vector2.zero);
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            EventManager<Vector2>.Instance.TriggerEvent("rightMovement", Vector2.right);
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
        {
            EventManager<Vector2>.Instance.TriggerEvent("rightMovement", Vector2.zero);
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            EventManager<bool>.Instance.TriggerEvent("jumpMovement",true);
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
        {
            EventManager<bool>.Instance.TriggerEvent("jumpMovement", false);
        }
        if (Input.GetKey(KeyCode.Space) || Input.GetKey(KeyCode.P))
        {
            //EventManager<bool>.Instance.TriggerEvent("createTentacle", true);
            //EventManager<bool>.Instance.TriggerEvent("dashMovement", true);
            EventManager<bool>.Instance.TriggerEvent("attac", true);
        }
    }
}
