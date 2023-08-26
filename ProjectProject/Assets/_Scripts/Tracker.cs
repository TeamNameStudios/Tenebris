using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tracker : Singleton<Tracker>
{
    [SerializeField] public float distance;
    private float maxX = -Mathf.Infinity;
    private float velocity;

    private void OnEnable()
    {
        EventManager<float>.Instance.StartListening("onPlayerChangeXVelociy", Move);
    }

    protected override void Awake()
    {
        base.Awake();
        
        distance = 0;
        transform.position = new Vector3(0, 0, 0);
    }

    private void Update()
    {
        Vector2 pos = transform.position;
        pos.x -= velocity;
        transform.position = pos;

        float tempPos = transform.position.x;
        distance = Mathf.RoundToInt(0 - tempPos);
    
        if (distance > maxX)
        {
            maxX = distance;
        }
    
        EventManager<float>.Instance.TriggerEvent("UpdateDistanceCount", maxX);
    }

    private void Move(float _velocity)
    {
        velocity = _velocity;

    }

    // DISTANCE UPDATED REAL TIME
    //private void Update()
    //{
    //    distance = Mathf.Round(0 - transform.position.x) * 0.01f;
    //    float realDistance = Mathf.Clamp(distance, 0f, float.PositiveInfinity);
    //    EventManager<float>.Instance.TriggerEvent("UpdateDistanceCount", realDistance);
    //
    //    if (distance >= 9.70)
    //    {
    //        EventManager<bool>.Instance.TriggerEvent("onLevelEnded", true);
    //    }
    //}
}
