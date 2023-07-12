using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : Manifestation
{
    [SerializeField] private float velocity;
    [SerializeField] private Vector2 leftCorner;
    [SerializeField] private Vector2 rightCorner;

    [SerializeField] private bool canStart = false;

    private void Update()
    {
        CanStart();
    }

    private void FixedUpdate()
    {
        if (canStart)
        {
            Vector2 pos = transform.position;
            
            if (playerDirection.x < 0)
            {
                pos.x -= Vector2.right.x * velocity * Time.fixedDeltaTime;
            }
            else
            {
                float finalVelocity = velocity + playerVelocity;
                pos.x -= Vector2.right.x * finalVelocity * Time.fixedDeltaTime;
            }
            
            transform.position = pos;
        }
    }

    private void CanStart()
    {
        if (!canStart)
        {
            Collider2D collider = Physics2D.OverlapArea(leftCorner, rightCorner);

            if (collider != null && collider.gameObject.CompareTag("Player"))
            {
                Vector2 pos = transform.position;
                pos.y = collider.transform.position.y;
                transform.position = pos;
                canStart = true;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine((Vector2)transform.position + leftCorner, (Vector2)transform.position + rightCorner);
        Gizmos.DrawLine((Vector2)transform.position + leftCorner, new Vector2(transform.position.x + leftCorner.x, transform.position.y + rightCorner.y));
    }
}
