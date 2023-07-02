using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;
public class PlayDash : PlayContro
{

    public AnimationCurve dashCurve;
    public float dashTimer;
    private float _dTimer;
    public float dashDistance;
    private bool _dashing;

    public float dashCoolDown;
    public float timerCD;

    public bool isDark;
    public enum blockedDirection
    {
        None,
        Left,
        Right
    }
    public blockedDirection blocked;
    public Collider2D coll2D;

    private bool dashing
    {
        get { return _dashing; }
        set
        {
            _dashing = value;
            velocity.y = 0;
        }
    }

    internal void MoveD(Vector2 movementDirection)
    {
        if (blocked == blockedDirection.Left)
            velocity.x = Mathf.Max(movementDirection.x, 0) * speed;
        else if (blocked == blockedDirection.Right)
            velocity.x = Mathf.Min(movementDirection.x, 0) * speed;
        else
            velocity.x = movementDirection.x * speed;

        EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", velocity.x);
    }

    private new void OnEnable()
    {
        coll2D = GetComponent<Collider2D>();

        base.OnEnable();
        EventManager<Vector2>.Instance.StopListening("leftMovement", Move);
        EventManager<Vector2>.Instance.StopListening("rightMovement", Move);
        EventManager<Vector2>.Instance.StartListening("leftMovement", MoveD);
        EventManager<Vector2>.Instance.StartListening("rightMovement", MoveD);
        EventManager<bool>.Instance.StartListening("dashMovement", Dash);
    }
    private new void OnDisable()
    {
        EventManager<bool>.Instance.StopListening("jumpMovement", Jump);
        EventManager<Vector2>.Instance.StopListening("leftMovement", MoveD);
        EventManager<Vector2>.Instance.StopListening("rightMovement", MoveD);
        EventManager<bool>.Instance.StopListening("dashMovement", Dash);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("dashObstacle") && !isDark)
        {
            if (collision.transform.position.x >= transform.position.x)
                blocked = blockedDirection.Right;
            else
                blocked = blockedDirection.Left;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("dashObstacle"))
            blocked = blockedDirection.None;
    }

    private void Dash(bool isDashing)
    {
        if(timerCD <= 0)
            dashing = true;

        if (dashing && !isDark)
        {
            isDark = true;
            _dTimer = 0;
            EventManager<Vector2>.Instance.StopListening("leftMovement", MoveD);
            EventManager<Vector2>.Instance.StopListening("rightMovement", MoveD);
            EventManager<bool>.Instance.StopListening("jumpMovement", Jump);
        }
    }

    private new void FixedUpdate()
    {
        if (dashing)
        {
            velocity.y = 0;

            _dTimer += Time.fixedDeltaTime;
            if (_dTimer < dashTimer)
            {
                float t = dashCurve.Evaluate(_dTimer);
                float lerp = Mathf.Lerp(0, dashDistance, t);
                lerp = velocity.x >= 0 ? lerp : -lerp;
                EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", lerp);
            }
            else
            {
                isDark = false;
                dashing = false;
                timerCD = dashCoolDown;
                EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", 0);
                EventManager<Vector2>.Instance.StartListening("leftMovement", MoveD);
                EventManager<Vector2>.Instance.StartListening("rightMovement", MoveD);
                EventManager<bool>.Instance.StartListening("jumpMovement", Jump);
            }
        }

        if (timerCD > 0)
            timerCD -= Time.fixedDeltaTime;

        base.FixedUpdate();
    }
}
