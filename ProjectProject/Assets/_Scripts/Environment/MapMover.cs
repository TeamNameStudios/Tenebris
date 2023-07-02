using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMover : MonoBehaviour
{
    public float depth = 1;

    private float velocity;

    private void OnEnable()
    {
        EventManager<float>.Instance.StartListening("onPlayerChangeXVelociy",changeVelocity);
    }

    private void OnDisable()
    {
        EventManager<float>.Instance.StopListening("onPlayerChangeXVelociy", changeVelocity);
    }

    private void changeVelocity(float _velocity)
    {
        velocity = _velocity;  
    }

    public void FixedUpdate()
    {
        float realVelocity = velocity / depth;
        Vector2 pos = transform.position;
        pos.x -= realVelocity * Time.fixedDeltaTime;
        transform.position = pos;
    }
}
