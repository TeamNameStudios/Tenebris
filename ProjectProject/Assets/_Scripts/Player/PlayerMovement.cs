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
        if (player.isDashing || playerHook.isHooked)
        {
            return;
        }
        if (player.direction.x == 0f)
        {
            player.velocity.x = Mathf.MoveTowards(player.velocity.x, 0, deAcceleration * Time.deltaTime);
        }
        else
        {
            if (Math.Abs(player.velocity.x) > maxVelocity) 
            {
                player.velocity.x = Mathf.MoveTowards(player.velocity.x, maxVelocity * player.direction.x, deAcceleration * Time.deltaTime);
            }
            else
            {
                player.velocity.x += acceleration * player.direction.x * Time.deltaTime;
                player.velocity.x = Math.Clamp(player.velocity.x, -maxVelocity, maxVelocity);

            }
        }
        EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", player.velocity.x);
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
}
