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
        InputController.OnLeftMovement += Move;
        InputController.OnRightMovement += Move;
        InputController.OnJumpMovement += Jump;
    }
    private void OnDisable()
    {
        InputController.OnLeftMovement -= Move;
        InputController.OnRightMovement -= Move;
        InputController.OnJumpMovement -= Jump;
    }

    private void Move(Vector2 movementDirection)
    {

        if (isGrounded)
        {
            velocity.x = movementDirection.x * speed;
        }
    }
        private void Jump()
    {
        if (isGrounded)
        {
            isGrounded = false;
            velocity.y = jumpVelocity;
        }
    }

    private void Update()
    {
        //if (isGrounded)
        //{
        //    if(Input.GetKeyDown(KeyCode.Space)) { 
        //        isGrounded = false;
        //        velocity.y = jumpVelocity;
        //    }
        //}
    }

    private void FixedUpdate()
    {
        Vector2 pos = transform.position;
        if(!isGrounded)
        {
            pos.y += velocity.y * Time.fixedDeltaTime;
            velocity.y += gravity * Time.fixedDeltaTime;

            if(pos.y <= groundHeight) {
                pos.y = groundHeight;
                isGrounded=true;
            }
        }

        transform.position = pos;
    }
}
