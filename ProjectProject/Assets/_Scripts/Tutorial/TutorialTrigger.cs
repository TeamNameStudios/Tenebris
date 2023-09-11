using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{
    [SerializeField] private Vector2 boxSize;

    private bool playerFound = false;
    [SerializeField] private bool lastTutorial = false;

    [SerializeField] private string dialogueName;

    private void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, boxSize, 0);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponent<Player>() && !playerFound)
            {
                if (!lastTutorial)
                {
                    playerFound = true;
                    EventManager<GameState>.Instance.TriggerEvent("onStateChanged", GameState.TUTORIAL);
                    EventManager<string>.Instance.TriggerEvent("onPlayDialogue", dialogueName);
                    Destroy(gameObject);
                }
                else
                {
                    playerFound = true;
                    EventManager<bool>.Instance.TriggerEvent("onTutorialEnd", true);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.DrawWireCube(transform.position, boxSize);
    //    Gizmos.DrawLine(transform.position, new Vector3(spawnPosX + transform.position.x, transform.position.y, 0));
    //}
}
