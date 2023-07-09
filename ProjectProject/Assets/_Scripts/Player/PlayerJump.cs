using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;
using static UnityEditor.PlayerSettings;

public class PlayerJump : MonoBehaviour
{
    [SerializeField]
    public float jumpVelocity = 20;
    [SerializeField]
    public float groundHeight = 10;
    [SerializeField]
    public float originalGravity;
    [SerializeField]
    public float gravity;
    [SerializeField]
    private Player player;
    [SerializeField]
    private PlayerHook playerHook;
    private void Awake()
    {
        player = GetComponent<Player>();
        playerHook = GetComponent<PlayerHook>();
        originalGravity = gravity;
    }


    private void OnEnable()
    { 
        EventManager<bool>.Instance.StartListening("jumpMovement", Jump);
    }
    private void OnDisable()
    {
        EventManager<bool>.Instance.StopListening("jumpMovement", Jump);
    }

    private bool InputJump;

    private void Jump(bool _inputJump)
    {
        if (_inputJump)
        {
            _lastJumpPressed = Time.time;
        }
        InputJump = _inputJump;
    }

    private void FixedUpdate()
    {
        Vector2 pos = transform.position;
        CalculateJumpApex();
        player.CalculateGravity();
        CalculateJump();
        if (player.isDashing || playerHook.isHooked)
        {
            return;
        }
        pos.y += player.velocity.y * Time.fixedDeltaTime;

        transform.position = pos;

    }

    #region Jump

    [Header("JUMPING")]
    [SerializeField] public float _jumpHeight = 30;
    [SerializeField] public float _jumpApexThreshold = 10f;
    [SerializeField] public float _coyoteTimeThreshold = 0.1f;
    [SerializeField] public float _jumpBuffer = 0.1f;
    [SerializeField] public float _jumpEndEarlyGravityModifier = 3;
    public bool _coyoteUsable;
    public bool _endedJumpEarly = true;
    private float _apexPoint; // Becomes 1 at the apex of a jump
    private float _lastJumpPressed;
    private bool CanUseCoyote => _coyoteUsable && !player._colDown && player._timeLeftGrounded + _coyoteTimeThreshold > Time.time;
    private bool HasBufferedJump => player._colDown && _lastJumpPressed + _jumpBuffer > Time.time;

    public void CalculateJumpApex()
    {
        if (!player._colDown)
        {
            // Gets stronger the closer to the top of the jump
            _apexPoint = Mathf.InverseLerp(_jumpApexThreshold, 0, Mathf.Abs(player.velocity.y));
            player._fallSpeed = Mathf.Lerp(player._minFallSpeed, player._maxFallSpeed, _apexPoint);
        }
        else
        {
            _apexPoint = 0;
        }
    }

    public void CalculateJump()
    {
        // Jump if: grounded or within coyote threshold || sufficient jump buffer
        if (InputJump && CanUseCoyote || HasBufferedJump)
        {
            player.velocity.y = _jumpHeight;
            _endedJumpEarly = false;
            _coyoteUsable = false;
            player._timeLeftGrounded = float.MinValue;
            player.JumpingThisFrame = true;
        }
        else
        {
            player.JumpingThisFrame = false;
        }

        // End the jump early if button released
        if (!player._colDown && !InputJump && !_endedJumpEarly && player.velocity.y > 0)
        {
            // _currentVerticalSpeed = 0;
            _endedJumpEarly = true;
        }

        if (player._colUp)
        {
            if (player.velocity.y > 0) player.velocity.y = 0;
        }
    }

    #endregion

    //private void Jump(bool isJumping)
    //{
    //    if (player.isGrounded && isJumping)
    //    {
    //        player.isGrounded = false;
    //        player.velocity.y = jumpVelocity;
    //    }
    //}

    //private void FixedUpdate()
    //{
    //    Vector2 pos = transform.position;

    //    if (player.isDashing || playerHook.isHooked)
    //    {
    //        return;
    //    }
    //    if (!player.isGrounded)
    //    {
    //        pos.y += player.velocity.y * Time.fixedDeltaTime;
    //        player.velocity.y += gravity * Time.fixedDeltaTime;

    //        Vector2 raycastOrigin = new Vector2((pos.x + 0.7f) * player.direction.x, pos.y);
    //        Vector2 rayDirection = Vector2.up;
    //        float rayDistance = player.velocity.y * Time.fixedDeltaTime;
    //        RaycastHit2D hit2D = Physics2D.Raycast(raycastOrigin, rayDirection, rayDistance);
    //        if (hit2D.collider != null)
    //        {
    //            LandableGround landableGround = hit2D.collider.GetComponent<LandableGround>();
    //            if (landableGround != null)
    //            {
    //                groundHeight = landableGround.groundHeight;
    //                pos.y = groundHeight;
    //                player.velocity.y = 0f;
    //                player.isGrounded = true;
    //            }

    //            // To get the size of the tilemap !!!
    //            //TilemapCollider2D tilemapCollider = hit2D.collider.GetComponent<TilemapCollider2D>();
    //            //Tilemap tilemap = hit2D.collider.GetComponent<Tilemap>();
    //            //
    //            //if (tilemapCollider != null)
    //            //{
    //            //    tilemap.CompressBounds();
    //            //    Vector3Int tilemapSize = tilemap.size;
    //            //
    //            //    groundHeight = (tilemapCollider.transform.position.y + tilemapCollider.transform.localScale.y / 2) + tilemapSize.y;
    //            //    pos.y = groundHeight;
    //            //    player.velocity.y = 0f;
    //            //
    //            //    player.isGrounded = true;
    //            //}
    //        }
    //        Debug.DrawRay(raycastOrigin, rayDirection * rayDistance, Color.cyan);
    //    }
    //    if (player.isGrounded)
    //    {
    //        Vector2 raycastOrigin = new Vector2((pos.x - 0.7f) * player.direction.x, pos.y);
    //        Vector2 rayDirection = Vector2.up;
    //        float rayDistance = player.velocity.y * Time.fixedDeltaTime;
    //        RaycastHit2D hit2D = Physics2D.Raycast(raycastOrigin, rayDirection, rayDistance);
    //        if (hit2D.collider == null)
    //        {
    //            player.isGrounded = false;
    //        }
    //        Debug.DrawRay(raycastOrigin, rayDirection * rayDistance, Color.green);
    //    }
    //    transform.position = pos;
    //}
}
