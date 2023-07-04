using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapMover : MonoBehaviour
{
    public float depth = 1;
    [SerializeField]
    private float velocity;
    [SerializeField]
    private bool isDashing;
    private void OnEnable()
    {
        EventManager<float>.Instance.StartListening("onPlayerChangeXVelociy",changeVelocity);
        EventManager<bool>.Instance.StartListening("isDashing", IsDashing);
    }

    private void OnDisable()
    {
        EventManager<float>.Instance.StopListening("onPlayerChangeXVelociy", changeVelocity);
        EventManager<bool>.Instance.StartListening("isDashing", IsDashing);
    }

    private void changeVelocity(float _velocity)
    {
        velocity = _velocity;  
    }
    private void IsDashing(bool _isDashing)
    {
        isDashing = _isDashing;
    }

    public void FixedUpdate()
    {
        //if (isDashing)
        //{
        //    return;
        //}
        float realVelocity = velocity / depth;
        Vector2 pos = transform.position;
        pos.x -= realVelocity * Time.fixedDeltaTime;
        transform.position = pos;
    }
}
