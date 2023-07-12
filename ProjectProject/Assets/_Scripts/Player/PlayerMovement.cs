using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    public float maxVelocity;
    [SerializeField]
    public float acceleration;
    [SerializeField]
    public float deAcceleration;
    [SerializeField]
    public bool isFacingRight = true;
    [SerializeField]
    private Player player;
    [SerializeField]
    private PlayerHook playerHook;

    private void Awake()
    {
        player = GetComponent<Player>();
        playerHook = GetComponent<PlayerHook>();
    }

    private void OnEnable()
    {
        EventManager<Vector2>.Instance.StartListening("movement", Move);
    }
    private void OnDisable()
    {
        EventManager<Vector2>.Instance.StopListening("movement", Move);
    }

    private void Move(Vector2 movementDirection)
    {
        player.direction = movementDirection;
        Flip();
    }

    // Update is called once per frame
    void Update()
    {
        //if (player.isDashing || playerHook.isHooked)
        //{
        //    return;
        //}
        //if (player.direction.x == 0f)
        //{
        //    player.velocity.x = Mathf.MoveTowards(player.velocity.x, 0, deAcceleration);
        //}
        //else
        //{
        //    if (Math.Abs(player.velocity.x) > maxVelocity) 
        //    {
        //        player.velocity.x = Mathf.MoveTowards(player.velocity.x, maxVelocity * player.direction.x, deAcceleration);
        //    }
        //    else
        //    {
        //        player.velocity.x += acceleration * player.direction.x;
        //        player.velocity.x = Math.Clamp(player.velocity.x, -maxVelocity, maxVelocity);

        //    }
        //}
        //EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", player.velocity.x);
    }

    public void Flip()
    {
        if (isFacingRight && player.direction.x < 0f || !isFacingRight && player.direction.x > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    public void CalculateWalk()
    {
        if (player.direction.x != 0f)
        {
            // Set horizontal move speed
            player.velocity.x += acceleration * player.direction.x;

            // clamped by max frame movement
            player.velocity.x = Math.Clamp(player.velocity.x, -maxVelocity, maxVelocity);

            //// Apply bonus at the apex of a jump
            //var apexBonus = Mathf.Sign(player.direction.x) * _apexBonus * _apexPoint;
            //_currentHorizontalSpeed += apexBonus * Time.deltaTime;
        }
        else
        {
            // No input. Let's slow the character down
            player.velocity.x = Mathf.MoveTowards(player.velocity.x, maxVelocity * player.direction.x, deAcceleration);
            //_currentHorizontalSpeed = Mathf.MoveTowards(_currentHorizontalSpeed, 0, _deAcceleration * Time.deltaTime);
        }

        if (player.velocity.x > 0 && player._colRight || player.velocity.x < 0 && player._colLeft)
        {
            // Don't walk through walls
            player.velocity.x = 0;
        }
        EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", player.velocity.x);
    }
}
