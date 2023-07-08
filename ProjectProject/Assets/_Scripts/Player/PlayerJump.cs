using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class PlayerJump : MonoBehaviour
{
    [SerializeField]
    public float jumpVelocity = 20;
    [SerializeField]
    public float groundHeight = 10;
    [SerializeField]
    public float gravity;
    [SerializeField]
    private Player player;
    private void Awake()
    {
        player = GetComponent<Player>();
    }


    private void OnEnable()
    { 
        EventManager<bool>.Instance.StartListening("jumpMovement", Jump);
    }
    private void OnDisable()
    {
        EventManager<bool>.Instance.StopListening("jumpMovement", Jump);
    }

    private void Jump(bool isJumping)
    {
        if (player.isGrounded && isJumping)
        {
            player.isGrounded = false;
            player.velocity.y = jumpVelocity;
        }
    }

    private void FixedUpdate()
    {
        Vector2 pos = transform.position;

        if (player.isDashing)
        {
            return;
        }
        if (!player.isGrounded)
        {
            pos.y += player.velocity.y * Time.fixedDeltaTime;
            player.velocity.y += gravity * Time.fixedDeltaTime;

            Vector2 raycastOrigin = new Vector2((pos.x + 0.7f) * player.direction.x, pos.y);
            Vector2 rayDirection = Vector2.up;
            float rayDistance = player.velocity.y * Time.fixedDeltaTime;
            RaycastHit2D hit2D = Physics2D.Raycast(raycastOrigin, rayDirection, rayDistance);
            if (hit2D.collider != null)
            {
                LandableGround landableGround = hit2D.collider.GetComponent<LandableGround>();
                if (landableGround != null)
                {
                    groundHeight = landableGround.groundHeight;
                    pos.y = groundHeight;
                    player.velocity.y = 0f;
                    player.isGrounded = true;
                }

                // To get the size of the tilemap !!!
                //TilemapCollider2D tilemapCollider = hit2D.collider.GetComponent<TilemapCollider2D>();
                //Tilemap tilemap = hit2D.collider.GetComponent<Tilemap>();
                //
                //if (tilemapCollider != null)
                //{
                //    tilemap.CompressBounds();
                //    Vector3Int tilemapSize = tilemap.size;
                //
                //    groundHeight = (tilemapCollider.transform.position.y + tilemapCollider.transform.localScale.y / 2) + tilemapSize.y;
                //    pos.y = groundHeight;
                //    player.velocity.y = 0f;
                //
                //    player.isGrounded = true;
                //}
            }
            Debug.DrawRay(raycastOrigin, rayDirection * rayDistance, Color.cyan);
        }
        if (player.isGrounded)
        {
            Vector2 raycastOrigin = new Vector2((pos.x - 0.7f) * player.direction.x, pos.y);
            Vector2 rayDirection = Vector2.up;
            float rayDistance = player.velocity.y * Time.fixedDeltaTime;
            RaycastHit2D hit2D = Physics2D.Raycast(raycastOrigin, rayDirection, rayDistance);
            if (hit2D.collider == null)
            {
                player.isGrounded = false;
            }
            Debug.DrawRay(raycastOrigin, rayDirection * rayDistance, Color.green);
        }
        transform.position = pos;
    }
}
