using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;

public class PlayTentaclePlatform : PlayContro
{

    public TentaclePlatform tentacle;
    private bool _canCreateTentacle;
    private float initPose;
    private float endPose;
    public Collider2D coll2D;
    private bool isRising;
    public float corruptionNeeded = 10;


    private new void OnEnable() 
    {
        base.OnEnable();
        EventManager<bool>.Instance.StartListening("createTentacle", TentacleUp);
        EventManager<bool>.Instance.StartListening("finishTentacle", TentacleFinish);
        coll2D = GetComponent<Collider2D>();
    }
    private new void OnDisable()
    {
        base.OnDisable();
        EventManager<bool>.Instance.StopListening("createTentacle", TentacleUp);
        EventManager<bool>.Instance.StopListening("finishTentacle", TentacleFinish);
    }

    private void TentacleUp(bool sentinel)
    {
        if (corruptionNeeded > corruption)
            return;

        if (_canCreateTentacle && sentinel && isGrounded)
        {
            EventManager<float>.Instance.TriggerEvent("updateCorruption", -corruptionNeeded);
            _canCreateTentacle = false;
            Vector2 pos = new Vector2(
                transform.position.x,
                coll2D.bounds.min.y);

            tentacle.TentacleInit(pos);
        }
    }
    private void TentacleFinish(bool canCreateAgain)
    {
        _canCreateTentacle = canCreateAgain;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("TentaPlatform"))
        {
            isGrounded = true;
            Vector2 pos = transform.position;
            pos.y = collision.bounds.max.y + coll2D.bounds.extents.y;
            transform.position = pos;
            isRising = true;
            return;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("TentaPlatform"))
        {
            isGrounded = true;
            Vector2 pos = transform.position;
            pos.y = collision.bounds.max.y + coll2D.bounds.extents.y;
            transform.position = pos;
            isRising = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("TentaPlatform"))
        {
            isRising = false;
            isGrounded = false;
            velocity.y =  Mathf.Max(velocity.y, 0);
        }
    }

    private new void OnCollisionEnter2D(Collision2D collision)
    {
        if (isRising && isGrounded)
            return;
        base.OnCollisionEnter2D(collision);
    }

    private new void OnCollisionStay2D(Collision2D collision)
    {
        if (isRising && isGrounded)
            return;
        base.OnCollisionStay2D(collision);
    }

    private new void OnCollisionExit2D(Collision2D collision)
    {
        if (isRising && isGrounded)
            return;
        base.OnCollisionExit2D(collision);
    }
}
