using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;
public class Player : MonoBehaviour
{

    public float gravity;
    public Vector2 velocity;
    public Vector2 direction = Vector2.zero;
    public float maxVelocity;
    public float speed = 2f;
    public float acceleration;
    public float maxAcceleration;
    public float jumpVelocity =20;
    public float groundHeight = 10;
    public bool isGrounded = false;

    private void Awake()
    {
     
    }

    private void OnEnable()
    {
        EventManager<Vector2>.Instance.StartListening("leftMovement",Move);
        EventManager<Vector2>.Instance.StartListening("rightMovement", Move);
        EventManager<bool>.Instance.StartListening("jumpMovement", Jump);
    }
    private void OnDisable()
    {
        EventManager<Vector2>.Instance.StopListening("leftMovement", Move);
        EventManager<Vector2>.Instance.StopListening("rightMovement", Move);
        EventManager<bool>.Instance.StopListening("jumpMovement", Jump);
    }

    private void Move(Vector2 movementDirection)
    {
        velocity.x = movementDirection.x * speed;
        EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", velocity.x);
    }

    private void Jump(bool isJumping)
    {
        if (isGrounded && isJumping)
        {
            isGrounded = false;
            velocity.y = jumpVelocity;
        }
    }

    private void FixedUpdate()
    {
        Vector2 pos = transform.position;
        if (!isGrounded)
        {
            pos.y += velocity.y * Time.fixedDeltaTime;
            velocity.y += gravity * Time.fixedDeltaTime;

            Vector2 raycastOrigin = new Vector2((pos.x + 0.7f)* direction.x, pos.y);
            Vector2 rayDirection = Vector2.up;
            float rayDistance = velocity.y * Time.fixedDeltaTime;
            RaycastHit2D hit2D = Physics2D.Raycast(raycastOrigin, rayDirection, rayDistance);
            if(hit2D.collider != null)
            {
                LandableGround landableGround = hit2D.collider.GetComponent<LandableGround>();
                if(landableGround != null)
                {
                    groundHeight = landableGround.groundHeight;
                    pos.y = groundHeight;
                    velocity.y = 0f;
                    isGrounded = true;
                }
            }
            Debug.DrawRay(raycastOrigin, rayDirection * rayDistance, Color.cyan);
        }
        if (isGrounded)
        {
            Vector2 raycastOrigin = new Vector2((pos.x - 0.7f) * direction.x, pos.y);
            Vector2 rayDirection = Vector2.up;
            float rayDistance = velocity.y * Time.fixedDeltaTime;
            RaycastHit2D hit2D = Physics2D.Raycast(raycastOrigin, rayDirection, rayDistance);
            if (hit2D.collider == null)
            {
                isGrounded = false;
            }
            Debug.DrawRay(raycastOrigin, rayDirection * rayDistance, Color.green);
        }
        transform.position = pos;
    }
}
