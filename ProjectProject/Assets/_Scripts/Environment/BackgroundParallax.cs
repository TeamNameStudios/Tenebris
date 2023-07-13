using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallax : MapMover
{
    private new void Update()
    {
        base.Update();
        Vector2 pos = transform.position;
        if (pos.x <= -56)
        {
            pos.x = 55;
        }
        if (pos.x >= 56)
        {
            pos.x = -55;
        }
        transform.position = pos;


    }
}
