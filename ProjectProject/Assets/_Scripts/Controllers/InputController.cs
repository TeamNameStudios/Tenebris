using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : Singleton<InputController>
{
   
    //public static event Action<Vector2> OnLeftMovement;
    //public static event Action<Vector2> OnRightMovement;
    //public static event Action<bool> OnJumpMovement;
    private void Update()
    {

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            //OnLeftMovement?.Invoke(Vector2.left);
            EventManager.Instance.TriggerEvent("MoveLeft", Vector2.left);
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.A))
        {
            //OnLeftMovement?.Invoke(Vector2.zero);
            EventManager.Instance.TriggerEvent("MoveLeft", Vector2.zero);
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            //OnRightMovement?.Invoke(Vector2.right);
            EventManager.Instance.TriggerEvent("MoveRight", Vector2.right);
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.D))
        {
            //OnRightMovement?.Invoke(Vector2.zero);
            EventManager.Instance.TriggerEvent("MoveRight", Vector2.zero);
        }
        else if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            //OnJumpMovement?.Invoke(true);
            EventManager.Instance.TriggerEvent("Jump", true);
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
        {
            //OnJumpMovement?.Invoke(false);
            EventManager.Instance.TriggerEvent("Jump", false);
        }
    }
}
