using UnityEngine;

public class InputController : Singleton<InputController>
{
    private void Update()
    {
        if (GameController.Instance.State == GameState.PLAYING)
        {
            float directionX = Input.GetAxisRaw("Horizontal");
            float directionY = Input.GetAxisRaw("Vertical");
            EventManager<Vector2>.Instance.TriggerEvent("movement", new Vector2(directionX, directionY));

            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
            {
                EventManager<bool>.Instance.TriggerEvent("jumpMovement", true);
            }
            //else if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
            //{
            //    EventManager<bool>.Instance.TriggerEvent("jumpMovement", false);
            //}
            if (Input.GetKeyDown(KeyCode.Space))
            {
                EventManager<bool>.Instance.TriggerEvent("isDashing", true);
            }
            if (Input.GetKeyDown(KeyCode.LeftShift))
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
