using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BestDistanceMarker : MapMover
{
    private void Update()
    {
        base.Update();
        Vector3 pos = transform.position;
        pos.x -= velocity * Time.deltaTime;
        transform.position = pos;
    }
}
