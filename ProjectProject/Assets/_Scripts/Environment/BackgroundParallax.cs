using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundParallax : MapMover
{
    private new void Update()
    {
        base.Update();
        Vector2 pos = transform.position;
        if (pos.x <= -70)
        {
            pos.x = 69;
        }
        if (pos.x >= 70)
        {
            pos.x = -69;
        }
        transform.position = pos;


    }
}
