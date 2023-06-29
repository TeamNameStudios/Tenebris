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
        //InputController.OnLeftMovement += Move;
        //InputController.OnRightMovement += Move;
        //InputController.OnJumpMovement += Jump;

        EventManager.Instance.StartListening("MoveLeft", Move);
        EventManager.Instance.StartListening("MoveRight", Move);
        EventManager.Instance.StartListening("Jump", Jump);
    }
    private void OnDisable()
    {
        //InputController.OnLeftMovement -= Move;
        //InputController.OnRightMovement -= Move;
        //InputController.OnJumpMovement -= Jump;

        EventManager.Instance.StopListening("MoveLeft", Move);
        EventManager.Instance.StopListening("MoveRight", Move);
        EventManager.Instance.StopListening("Jump", Jump);
    }
    
    //private void Move(Vector2 movementDirection)
    //{
    //     velocity.x = movementDirection.x * speed;
    //}

    private void Move(object movementDirection)
    {
        Vector2 moveDir = (Vector2)movementDirection;
        velocity.x = moveDir.x * speed;
    }

    //private void Jump(bool isJumping)
    //{
    //    if (isGrounded && isJumping)
    //    {
    //        isGrounded = false;
    //        velocity.y = jumpVelocity;
    //    }
    //}

    private void Jump(object isJumping)
    {
        if (isGrounded && (bool)isJumping)
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
                Terrain terrain = hit2D.collider.GetComponent<Terrain>();
                if(terrain != null)
                {
                    groundHeight = terrain.terrainHeight;
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
