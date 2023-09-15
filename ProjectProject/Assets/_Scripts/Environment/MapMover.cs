using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MapMover : MonoBehaviour
{
    public float depth = 1;
    [SerializeField]
    protected float velocity;
	
    protected virtual void OnEnable()
    {
        EventManager<float>.Instance.StartListening("onPlayerChangeXVelociy",changeVelocity);
    }

    protected virtual void OnDisable()
    {
        EventManager<float>.Instance.StopListening("onPlayerChangeXVelociy", changeVelocity);
    }

    private void changeVelocity(float _velocity)
    {
        velocity = _velocity;  
    }

    public virtual void Update()
    {
        float realVelocity = velocity / depth;
        Vector2 pos = transform.position;
        pos.x -= realVelocity * Time.deltaTime;
        transform.position = pos;
    }
}
