using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialInputController : Singleton<TutorialInputController>
{
    private bool dashTutorialPlayed = false;


    private void Update()
    {
        if (TutorialGameController.Instance.State == GameState.PLAYING)
        {
            float directionX = Input.GetAxisRaw("Horizontal");
            float directionY = Input.GetAxisRaw("Vertical");
            EventManager<Vector2>.Instance.TriggerEvent("movement", new Vector2(directionX, directionY));

            //if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
            //{
            //    EventManager<bool>.Instance.TriggerEvent("jumpMovement", true);
            //}
            //else if (Input.GetKeyUp(KeyCode.UpArrow) || Input.GetKeyUp(KeyCode.W))
            //{
            //    EventManager<bool>.Instance.TriggerEvent("jumpMovement", false);
            //}

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
                EventManager<bool>.Instance.TriggerEvent("isDashing", true);
            }
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                EventManager<bool>.Instance.TriggerEvent("isGrappling", true);
            }


        }

        if (Input.GetKeyUp(KeyCode.Escape) && TutorialGameController.Instance.State == GameState.PLAYING)
        {
            EventManager<bool>.Instance.TriggerEvent("pause", true);
        }
        else if (Input.GetKeyUp(KeyCode.Escape) && TutorialGameController.Instance.State == GameState.PAUSING)
        {
            EventManager<bool>.Instance.TriggerEvent("pause", false);
        }


        if (TutorialGameController.Instance.state == GameState.PLAYING && (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W)) && !dashTutorialPlayed && TutorialGameController.Instance.welcomeTutorialPlayed)
        {
            dashTutorialPlayed = true;
            EventManager<string>.Instance.TriggerEvent("onPlayDialogue", "dash");
            EventManager<GameState>.Instance.TriggerEvent("onStateChanged", GameState.TUTORIAL);
        }
    }
}