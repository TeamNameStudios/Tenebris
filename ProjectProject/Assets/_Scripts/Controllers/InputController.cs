using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputController : Singleton<InputController>
{
   
    public static event Action<Vector2> OnLeftMovement;
    public static event Action<Vector2> OnRightMovement;
    public static event Action<bool> OnJumpMovement;
    private void Update()
    {

        if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            OnLeftMovement?.Invoke(Vector2.left);
        }
        else if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A))
        {
            OnLeftMovement?.Invoke(Vector2.zero);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            OnRightMovement?.Invoke(Vector2.right);
        }
        else if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D))
        {
            OnRightMovement?.Invoke(Vector2.zero);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            OnJumpMovement?.Invoke(true);
        }
        else if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
        {
            OnJumpMovement?.Invoke(false);
        }
    }
}
