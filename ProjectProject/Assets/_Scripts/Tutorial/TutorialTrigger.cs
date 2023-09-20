using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private Vector2 boxSize;

    private bool playerFound = false;
    [SerializeField] private bool lastTutorial = false;
    [SerializeField] private bool corrupionTutorial = false;
    [SerializeField] private bool stopTutorialMusic = false;

    [SerializeField] private string dialogueName;

    private void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, boxSize, 0);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponent<Player>() && !playerFound)
            {
                //if (!lastTutorial)
                //{
                //    playerFound = true;
                //    EventManager<GameState>.Instance.TriggerEvent("onStateChanged", GameState.TUTORIAL);
                //    EventManager<string>.Instance.TriggerEvent("onPlayDialogue", dialogueName);
                //    Destroy(gameObject);
                //}
                //else
                //{
                //    playerFound = true;
                //    EventManager<bool>.Instance.TriggerEvent("onTutorialEnd", true);
                //}
                playerFound = true;

                if (lastTutorial)
                {
                    EventManager<bool>.Instance.TriggerEvent("onTutorialEnd", true);
                }
                else if (corrupionTutorial)
                {
                    EventManager<float>.Instance.TriggerEvent("Corruption", GameController.Instance.Player.MaxCorruption);
                    EventManager<GameState>.Instance.TriggerEvent("onStateChanged", GameState.TUTORIAL);
                    EventManager<string>.Instance.TriggerEvent("onPlayDialogue", dialogueName);
                }
                else if (stopTutorialMusic)
                {
                    EventManager<bool>.Instance.TriggerEvent("onStopMusic", true);
                    EventManager<GameState>.Instance.TriggerEvent("onStateChanged", GameState.TUTORIAL);
                    EventManager<string>.Instance.TriggerEvent("onPlayDialogue", dialogueName);
                }
                else
                {
                    EventManager<GameState>.Instance.TriggerEvent("onStateChanged", GameState.TUTORIAL);
                    EventManager<string>.Instance.TriggerEvent("onPlayDialogue", dialogueName);
                }
                
                Destroy(gameObject);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }
}
