using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : Singleton<InputController>
{

    private void Update()
    {

        float direction = Input.GetAxisRaw("Horizontal");
        EventManager<float>.Instance.TriggerEvent("movement", direction);

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
    }
   }
