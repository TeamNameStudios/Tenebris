using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : Manifestation
{
    [SerializeField] private float velocity;



    private void FixedUpdate()
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
