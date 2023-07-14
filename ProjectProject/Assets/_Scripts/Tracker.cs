using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : MonoBehaviour
{
    [SerializeField] public float distance;
    private float maxX = -Mathf.Infinity;

    //private void Update()
    //{
    //    float tempPos = transform.position.x;
    //    distance = Mathf.Round(0 - tempPos) * 0.01f;
    //
    //    if (distance > maxX)
    //    {
    //        maxX = distance;
    //    }
    //
    //    EventManager<float>.Instance.TriggerEvent("UpdateDistanceCount", maxX);
    //}

    // DISTANCE UPDATED REAL TIME
    private void Update()
    {
        distance = Mathf.Round(0 - transform.position.x) * 0.01f;
        float realDistance = Mathf.Clamp(distance, 0f, float.PositiveInfinity);
        EventManager<float>.Instance.TriggerEvent("UpdateDistanceCount", realDistance);

        if (distance >= 9.70)
        {
            EventManager<bool>.Instance.TriggerEvent("onLevelEnded", true);
        }
    }
}
