using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckOverlap : MonoBehaviour
{
    [SerializeField] private Vector2 boxSize;
    [SerializeField] private float groundHeight;

    private bool grounded = false;

    private void OnEnable()
    {
        EventManager<bool>.Instance.StartListening("jumpMovement", Grounded);
    }

    private void Update()
    {
        CheckCollisions();


    }
    private void CheckCollisions()
    {
        if (!grounded)
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, boxSize, 0);

            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].gameObject.layer == 3)
                {
                    groundHeight = colliders[i].ClosestPoint(transform.position).y;

                    transform.position = new Vector2(0, groundHeight);

                    grounded = true;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }

    private void Grounded(bool state)
    {
        grounded = false;
    }
}
