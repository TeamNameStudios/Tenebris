using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : Manifestation
{
    [SerializeField] private float velocity;
    [SerializeField] private Vector2 boxSize;

    [SerializeField] private bool canStart = false;

    private void Update()
    {
        if (!canStart)
        {
            CanStart();
        }
    }

    private void FixedUpdate()
    {
        if (canStart)
        {
            Vector2 pos = transform.position;
            
            pos.x -= Vector2.right.x * velocity * Time.fixedDeltaTime;
            
            transform.position = pos;
        }
    }

    private void CanStart()
    {
        if (!canStart)
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, boxSize, 0);
            
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].GetComponent<Player>())
                {
                    Vector2 pos = transform.position;
                    pos.y = colliders[i].transform.position.y;
                    transform.position = pos;
                    canStart = true;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(boxSize.x, boxSize.y, 0));
    }
}
