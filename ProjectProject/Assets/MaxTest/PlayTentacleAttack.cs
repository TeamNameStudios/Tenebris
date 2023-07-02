using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayTentacleAttack : Player
{
    public Collider2D coll2D;

    public TentacleAttack tentAtac;
    private bool _canAtac = true;

    private float initialVel;
    private float finalVel;
    private bool _hooked = false;
    public bool hooked
    {
        get { return _hooked; }
        set
        {
            _hooked = value;

        }
    }

    public AnimationCurve hookAnim;
    private float _hooTimer;

    private float hooTimer
    {
        get { return _hooTimer; }
        set
        {
            _hooTimer = value;
            if(_hooTimer <= 0)
            {
                EventManager<Vector2>.Instance.StartListening("leftMovement", Move);
                EventManager<Vector2>.Instance.StartListening("rightMovement", Move);
                EventManager<bool>.Instance.StartListening("jumpMovement", Jump);
            }
        }
    }

    private float hookCurrentTimer;
    public float preHookMaxVel;
    public float postHookVel;
    public float postHookDecel = 9 / 10;
    public float hookJump;

    private new void OnEnable()
    {
        base.OnEnable();
        EventManager<bool>.Instance.StartListening("attac", Attack);
        EventManager<Vector2>.Instance.StartListening("hook", Hook);
        EventManager<bool>.Instance.StartListening("atkAgn", AtacAgain);
        coll2D = GetComponent<Collider2D>();
    }
    private new void OnDisable()
    {
        base.OnDisable();
        EventManager<bool>.Instance.StopListening("attac", Attack);
        EventManager<Vector2>.Instance.StopListening("hook", Hook);
        EventManager<bool>.Instance.StopListening("atkAgn", AtacAgain);
    }

    private void Attack(bool sentinel)
    {
        if (_canAtac && sentinel)
        {
            _canAtac = false;
            tentAtac.TentacleInit(transform.position);
        }
    }

    private void AtacAgain(bool canAtk)
    {
        _canAtac = true;
    }

    private void Hook(Vector2 hitCollision)
    {
        hooked = true;

        velocity.y = 0;
        initialVel = velocity.x;
        finalVel = preHookMaxVel;

        hooTimer = tentAtac.currentTimer;
        hookCurrentTimer = 0;

        EventManager<Vector2>.Instance.StopListening("leftMovement", Move);
        EventManager<Vector2>.Instance.StopListening("rightMovement", Move);
        EventManager<bool>.Instance.StopListening("jumpMovement", Jump);
    }

    internal new void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
        var obj = collision.gameObject;
        if (obj.CompareTag("Hookable") &&
            transform.position.x >= obj.transform.position.x - float.Epsilon)
        {
            hooked = false;
            return;
        } 
    }
    internal new void OnCollisionEnter2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        var obj = collision.gameObject;
        if (obj.CompareTag("Hookable") &&
            transform.position.x >= obj.transform.position.x - float.Epsilon)
        {
            hooked = false;
            return;
        }
    }

    internal new void OnCollisionExit(Collision collision)
    {
        base.OnCollisionEnter(collision);
        var obj = collision.gameObject;
        if (obj.CompareTag("Hookable"))
            hooked = false; 
    }

    internal new void OnCollisionExit2D(Collision2D collision)
    {
        base.OnCollisionEnter2D(collision);
        var obj = collision.gameObject;
        if (obj.CompareTag("Hookable"))
            hooked = false;  
    }

    private new void FixedUpdate()
    {
        Vector2 pos = transform.position;
        if (hooked)
        {
            hookCurrentTimer += Time.fixedDeltaTime;

            if (hookCurrentTimer < hooTimer)
            {
                float t = hookAnim.Evaluate(hookCurrentTimer / hooTimer);
                float lerp = Mathf.Lerp(initialVel, finalVel, t);

                velocity.x = lerp;
                velocity.y = 0;
                EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", velocity.x);
            }
            else
            {
                velocity.x += postHookVel;
                velocity.y = hookJump;
                isGrounded = false;
                EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", velocity.x);
                hooked = false;
                tentAtac.hook = false;
            }
        }
        else if (hooTimer > 0)
            hooTimer = hooTimer - Time.fixedDeltaTime;
        
        if (!isGrounded)
        {
            pos.y += velocity.y * Time.fixedDeltaTime;
            velocity.y += gravity * Time.fixedDeltaTime;
        }

        if (velocity.x > 0)
        {
            velocity.x -= deceleration * Time.deltaTime;
            EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", velocity.x);
        }
        transform.position = pos;
    }
}
