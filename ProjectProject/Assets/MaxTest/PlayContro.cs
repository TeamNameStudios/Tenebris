using System;
using System.Collections;
using System.Collections.Generic;
//using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;
using TMPro;
public class PlayContro : MonoBehaviour
{

    public float gravity;
    public Vector2 velocity;
    public Vector2 direction = Vector2.zero;
    public float maxVelocity;
    public float deceleration;
    public float speed = 2f;
    public float acceleration;
    public float maxAcceleration;
    public float jumpVelocity = 20;
    public float groundHeight = 10;
    private bool _isGrounded;

    public float maxCorruption = 30;
    internal float corruption = 0;
    public TextMeshProUGUI corruptionUI;
    public CorruptionBar cBhori;
    public CorruptionBar cBverti;

    public bool isGrounded
    {
        get { return _isGrounded; }
        set
        {
            _isGrounded = value;
            if (isGrounded)
                velocity.y = 0;
        }
    }


    internal void Awake()
    {

    }

    internal void OnEnable()
    {
        EventManager<Vector2>.Instance.StartListening("leftMovement", Move);
        EventManager<Vector2>.Instance.StartListening("rightMovement", Move);
        EventManager<bool>.Instance.StartListening("jumpMovement", Jump);
        EventManager<float>.Instance.StartListening("updateCorruption", UpdateCorruption);
        cBhori.SetMaxValue(maxCorruption);
        cBverti.SetMaxValue(maxCorruption);
        UpdateCorruption(maxCorruption);
    }
    internal void OnDisable()
    {
        EventManager<Vector2>.Instance.StopListening("leftMovement", Move);
        EventManager<Vector2>.Instance.StopListening("rightMovement", Move);
        EventManager<bool>.Instance.StopListening("jumpMovement", Jump);
        EventManager<float>.Instance.StopListening("updateCorruption", UpdateCorruption);
    }

    internal void Move(Vector2 movementDirection)
    {
        velocity.x = movementDirection.x * speed;
        EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", velocity.x);
    }

    internal void Jump(bool isJumping)
    {
        if (isGrounded && isJumping)
        {
            isGrounded = false;
            velocity.y = jumpVelocity;
        }
    }

    internal void UpdateCorruption(float corrToAdd)
    {
        corruption += corrToAdd;

        cBhori.SetCorruptionBar(corruption);
        cBverti.SetCorruptionBar(corruption);

        corruptionUI.text = $"Corruption: {corruption}/{maxCorruption}";
    }

    internal void OnCollisionEnter(Collision collision)
    {
        var obj = collision.gameObject;
        var col = collision.collider;
        if (obj.CompareTag("Ground") &&
            transform.position.y >= col.bounds.max.y - float.Epsilon)
        {
            isGrounded = true;
        }
        else if (obj.CompareTag("Pushable") &&
            transform.position.y >= col.bounds.max.y - float.Epsilon)
        {
            isGrounded = true;
        }
    }

    internal void OnCollisionStay2D(Collision2D collision)
    {
        var obj = collision.gameObject;
        var col = collision.collider;
        if (obj.CompareTag("Ground") &&
            transform.position.y >= col.bounds.max.y - float.Epsilon)
        {
            isGrounded = true;
            Vector2 pos = transform.position;
            pos.y = collision.collider.bounds.max.y + 1;
            transform.position = pos;
        }
        else if (obj.CompareTag("Pushable") &&
            transform.position.y >= col.bounds.max.y - float.Epsilon)
        {
            isGrounded = true;
            Vector2 pos = transform.position;
            pos.y = collision.collider.bounds.max.y + 1;
            transform.position = pos;
        }
    }
    internal void OnCollisionEnter2D(Collision2D collision)
    {
        var obj = collision.gameObject;
        var col = collision.collider;
        if (obj.CompareTag("Ground") &&
            transform.position.y >= col.bounds.max.y - float.Epsilon)
        {
            isGrounded = true;
            velocity.y = 0;
        }
        else if (obj.CompareTag("Pushable") &&
            transform.position.y >= col.bounds.max.y - float.Epsilon)
        {
            isGrounded = true;
            Vector2 pos = transform.position;
            pos.y = collision.collider.bounds.max.y + 1;
            transform.position = pos;
        }
    }

    internal void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Pushable"))
            isGrounded = false;
    }

    internal void OnCollisionExit2D(Collision2D collision)
    {
        var obj = collision.gameObject;
        if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("Pushable"))
            isGrounded = false;
    }
    internal void FixedUpdate()
    {
        Vector2 pos = transform.position;

        if (!isGrounded)
        {
            pos.y += velocity.y * Time.fixedDeltaTime;
            velocity.y += gravity * Time.fixedDeltaTime;
        }
        transform.position = pos;
    }
}
