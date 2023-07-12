using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : Singleton<InputController>
{

    private void Update()
    {

        float directionX = Input.GetAxisRaw("Horizontal");
        float directionY = Input.GetAxisRaw("Vertical");
        EventManager<Vector2>.Instance.TriggerEvent("movement", new Vector2(directionX, directionY));

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            EventManager<bool>.Instance.TriggerEvent("jumpMovement", true);
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
        {
            EventManager<bool>.Instance.TriggerEvent("jumpMovement", false);
        }

        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            EventManager<bool>.Instance.TriggerEvent("jumpMovement", true);
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
        {
            EventManager<bool>.Instance.TriggerEvent("jumpMovement", false);
        }
        if (Input.GetKey(KeyCode.Space))
        {
            EventManager<bool>.Instance.TriggerEvent("dash", true);
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            EventManager<bool>.Instance.TriggerEvent("hook", true);
        }
        else if(Input.GetKeyUp(KeyCode.Z))
        {
            EventManager<bool>.Instance.TriggerEvent("hook",false);
        }

        if (Input.GetKeyUp(KeyCode.P))
        {
            EventManager<bool>.Instance.TriggerEvent("pause", true);
        }
        if (Input.GetKeyUp(KeyCode.O))
        {
            EventManager<bool>.Instance.TriggerEvent("pause", false);
        }
    }
   }
