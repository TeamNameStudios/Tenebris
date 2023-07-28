using System;
using System.Collections;
using UnityEngine;



public class PlayerManager : MonoBehaviour
{

    [SerializeField]
    public Rigidbody2D rb;
    [SerializeField]
    public Vector2 velocity;
    [SerializeField]
    public Vector2 direction = Vector2.zero;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        RunCollisionChecks();
        CalculateJumpApex();
        CalculateGravity();
        HandleDashing();
        if (!isDashing)
        {
            CalculateJump();
            CalculateMovement();
        }
        PerformMovement();
        ManageCorruption();
    }

    private void OnEnable()
    {
        EventManager<Vector2>.Instance.StartListening("movement", Move);
        EventManager<bool>.Instance.StartListening("jumpMovement", Jump);
        EventManager<float>.Instance.StartListening("Corruption", AddCorruption);
        EventManager<float>.Instance.StartListening("onCollectiblePickup", DecreaseCorruption);
        EventManager<bool>.Instance.StartListening("dash", Dash);
    }
    private void OnDisable()
    {
        EventManager<Vector2>.Instance.StopListening("movement", Move);
        EventManager<bool>.Instance.StopListening("jumpMovement", Jump);
        EventManager<float>.Instance.StopListening("Corruption", AddCorruption);
        EventManager<float>.Instance.StopListening("onCollectiblePickup", DecreaseCorruption);
        EventManager<bool>.Instance.StopListening("dash", Dash);
    }
    #region Detection

    [Header("Detection")]
    [SerializeField] 
    private LayerMask _groundMask;
    [SerializeField] 
    private float _grounderOffset = -1, _grounderRadius = 0.2f;
    [SerializeField] 
    private float _wallCheckOffset = 0.5f, _wallCheckRadius = 0.05f;
    [SerializeField]
    private bool _isAgainstLeftWall, _isAgainstRightWall, _isAgainstRoof;
    public bool IsGrounded;
    public float _timeLeftGrounded;

    private readonly Collider2D[] _ground = new Collider2D[1];
    private readonly Collider2D[] _leftWall = new Collider2D[1];
    private readonly Collider2D[] _rightWall = new Collider2D[1];
    private readonly Collider2D[] _upWall = new Collider2D[1];
    private void RunCollisionChecks()
    {
        bool grounded = Physics2D.OverlapCircleNonAlloc(transform.position + new Vector3(0, _grounderOffset), _grounderRadius, _ground, _groundMask) > 0;

        if (!IsGrounded && grounded) 
        {
            IsGrounded = true;
            _coyoteUsable = true;
        }
        else if (IsGrounded && !grounded)
        {
            _timeLeftGrounded = Time.time;
            IsGrounded = false;
        }

        _isAgainstRoof = Physics2D.OverlapCircleNonAlloc(transform.position + new Vector3(0, -_grounderOffset), _grounderRadius, _upWall, _groundMask) > 0;


        bool _isAgainstLeftWall1 = Physics2D.OverlapCircleNonAlloc(transform.position + new Vector3(-_wallCheckOffset, 0.75f), _wallCheckRadius, _leftWall, _groundMask) > 0;
        bool _isAgainstLeftWall2 = Physics2D.OverlapCircleNonAlloc(transform.position + new Vector3(-_wallCheckOffset, 0), _wallCheckRadius, _leftWall, _groundMask) > 0;
        bool _isAgainstLeftWall3 = Physics2D.OverlapCircleNonAlloc(transform.position + new Vector3(-_wallCheckOffset, -0.75f), _wallCheckRadius, _leftWall, _groundMask) > 0;
        bool _isAgainstRightWall1 = Physics2D.OverlapCircleNonAlloc(transform.position + new Vector3(_wallCheckOffset, 0.75f), _wallCheckRadius, _rightWall, _groundMask) > 0;
        bool _isAgainstRightWall2 = Physics2D.OverlapCircleNonAlloc(transform.position + new Vector3(_wallCheckOffset, 0), _wallCheckRadius, _rightWall, _groundMask) > 0;
        bool _isAgainstRightWall3 = Physics2D.OverlapCircleNonAlloc(transform.position + new Vector3(_wallCheckOffset, -0.75f), _wallCheckRadius, _rightWall, _groundMask) > 0;
        _isAgainstLeftWall = _isAgainstLeftWall1 || _isAgainstLeftWall2 || _isAgainstLeftWall3;
        _isAgainstRightWall = _isAgainstRightWall1 || _isAgainstRightWall2 || _isAgainstRightWall3;

    }

   
    private void DrawGrounderGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, _grounderOffset), _grounderRadius);
    }

    private void OnDrawGizmos()
    {
        DrawGrounderGizmos();
        DrawWallGizmos();
    }

    private void DrawWallGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(-_wallCheckOffset, 0.75f), _wallCheckRadius);
        Gizmos.DrawWireSphere(transform.position + new Vector3(-_wallCheckOffset, 0), _wallCheckRadius);
        Gizmos.DrawWireSphere(transform.position + new Vector3(-_wallCheckOffset, -0.75f), _wallCheckRadius);

        Gizmos.DrawWireSphere(transform.position + new Vector3(_wallCheckOffset, 0.75f), _wallCheckRadius);
        Gizmos.DrawWireSphere(transform.position + new Vector3(_wallCheckOffset, 0), _wallCheckRadius);
        Gizmos.DrawWireSphere(transform.position + new Vector3(_wallCheckOffset, -0.75f), _wallCheckRadius);

        Gizmos.DrawWireSphere(transform.position + new Vector3(0, -_grounderOffset), _grounderRadius);
    }

    #endregion

    #region Gravity

    [Header("GRAVITY")][SerializeField] private float _fallClamp = -40f;
    [SerializeField] public float _minFallSpeed = 80f;
    [SerializeField] public float _maxFallSpeed = 120f;
    public float _fallSpeed;


    public void CalculateGravity()
    {
        if (IsGrounded)
        {
            // Move out of the ground
            if (velocity.y < 0) velocity.y = 0;
        }
        else
        {
            if (!isDashing)
            {
                // Fall
                velocity.y -= _fallSpeed * Time.deltaTime;

                // Clamp
                if (velocity.y < _fallClamp) velocity.y = _fallClamp;
            }
        }
    }

    #endregion

    #region Movement
    [Header("MOVEMENT")]
    [SerializeField]
    public bool isFacingRight = true;
    [SerializeField]
    public float maxVelocity;
    [SerializeField]
    public float acceleration;
    [SerializeField]
    public float deAcceleration;
    private void Move(Vector2 movementDirection)
    {
        direction = movementDirection;
        Flip();
    }


    public void Flip()
    {
        if (/*!isDashing &&*/ isFacingRight && direction.x < 0f || !isFacingRight && direction.x > 0f)
        {
            isFacingRight = !isFacingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    public void CalculateMovement()
    {
        if (direction.x == 0f)
        {
            velocity.x = Mathf.MoveTowards(velocity.x, 0, deAcceleration * Time.deltaTime);
        }
        else
        {
            if (Math.Abs(velocity.x) > maxVelocity)
            {
                velocity.x = Mathf.MoveTowards(velocity.x, maxVelocity * direction.x, deAcceleration * Time.deltaTime);
            }
            else
            {
                velocity.x += acceleration * direction.x * Time.deltaTime;
                velocity.x = Math.Clamp(velocity.x, -maxVelocity, maxVelocity);
            }
        }

        if (velocity.x > 0 && _isAgainstRightWall || velocity.x < 0 && _isAgainstLeftWall)
        {
            // Don't walk through walls
            velocity.x = 0;
        }
       
    }

    #endregion

    #region Jump

    [Header("JUMPING")]
    [SerializeField] 
    public float _jumpHeight = 30;
    [SerializeField] 
    public float _jumpApexThreshold = 10f;
    [SerializeField] 
    public float _coyoteTimeThreshold = 0.1f;
    [SerializeField]
    public bool _coyoteUsable;
    private float _apexPoint; // Becomes 1 at the apex of a jump

    [SerializeField]
    private bool CanUseCoyote => _coyoteUsable && !IsGrounded && _timeLeftGrounded + _coyoteTimeThreshold > Time.time;
    [SerializeField]
    private bool InputJump;
    private void Jump(bool _inputJump)
    {
        InputJump = _inputJump;
    }

    public void CalculateJumpApex()
    {
        if (!IsGrounded && !isDashing)
        {
            // Gets stronger the closer to the top of the jump
            _apexPoint = Mathf.InverseLerp(_jumpApexThreshold, 0, Mathf.Abs(velocity.y));
            _fallSpeed = Mathf.Lerp(_minFallSpeed, _maxFallSpeed, _apexPoint);
        }
        else
        {
            _apexPoint = 0;
        }
    }

    public void CalculateJump()
    {
        // Jump if: grounded or within coyote threshold || sufficient jump buffer
        
        if (InputJump && IsGrounded || CanUseCoyote && InputJump)
        {
            velocity.y = _jumpHeight;
            _coyoteUsable = false;
            _timeLeftGrounded = float.MinValue;
            
            
           
            //JumpingThisFrame = true;
        }
        else
        {
            //JumpingThisFrame = false;
        }


        if (_isAgainstRoof)
        {
            if (velocity.y > 0) velocity.y = 0;
        }
    }
    #endregion

    #region Move
    private void PerformMovement()
    {
        Vector2 pos = transform.position;
        pos.y += velocity.y * Time.deltaTime;
        transform.position = pos;
        EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", velocity.x);
    }
    #endregion

    #region Corrution
    [Header("CORRUPTION_SYSTEM")]
    [SerializeField]
    private float Corruption;
    [SerializeField]
    private float maxCorruption;
    [SerializeField]
    private bool corrupted = false;
    [SerializeField]
    private bool invincibility = false;
    [SerializeField] 
    private float fullyCorruptionTime;
    [SerializeField] 
    private bool canRecover;
    [SerializeField]
    private float recoverCorruptionWaitTime;
    [SerializeField] 
    private float recoverCorruptionSpeed;
    [SerializeField]
    private float invincibilitySeconds;

    [Tooltip("Used to add a value to the recovered corruption, to not make it start from 0,\nKeep the value REALLY small, example: 0.02f")]
    [SerializeField]
    private float recoveryStartingSmoothness;

    public float elapsedTime = 0;
    public bool isRecovering = false;

    private Coroutine recoveryCO;
    private Coroutine corruptionCO;

    private void ManageCorruption()
    {
        if (!corrupted && canRecover && TempGameController.Instance.State == GameState.PLAYING)
        {
            if (Corruption > 0)
            {
                isRecovering = true;
                float normalizedTime = (elapsedTime / maxCorruption) + recoveryStartingSmoothness;
                float easeTime = EaseInCubic(normalizedTime);
                DecreaseCorruption(recoverCorruptionSpeed * easeTime);
                elapsedTime += Time.deltaTime;
                Debug.Log("Recovering " + normalizedTime * recoverCorruptionSpeed + " corruption");
            }
        }
    }
    private void AddCorruption(float value)
    {
        if (recoveryCO != null)
        {
            StopCoroutine(recoveryCO);
        }

        EventManager<float>.Instance.TriggerEvent("InitCorruptionBar", maxCorruption);

        if (!corrupted)
        {
            if (value + Corruption >= maxCorruption)
            {
                Corruption = maxCorruption;
            }
            else
            {
                Corruption += value;
                canRecover = false;
                isRecovering = false;

                recoveryCO = StartCoroutine(CanRecovery());
            }

            EventManager<float>.Instance.TriggerEvent("UpdateCorruptionBar", Corruption);

            if (Corruption >= maxCorruption)
            {

                if (recoveryCO != null)
                {
                    StopCoroutine(recoveryCO);
                }

                isRecovering = false;
                invincibility = true;
                corrupted = true;
                corruptionCO = StartCoroutine(CorruptionCoroutine());
            }
        }
    }

    private void DecreaseCorruption(float value)
    {
        if (Corruption - value <= 0)
        {
            Corruption = 0;
            // maybe spawn a particle effect ??
        }
        else
        {
            Corruption -= value;
        }

        //stop corruption coroutine and reset corrupted state
        if (recoveryCO != null)
        {
            StopCoroutine(recoveryCO);
        }

        if (corruptionCO != null)
        {
            StopCoroutine(corruptionCO);
            corrupted = false;
            invincibility = false;
            recoveryCO = StartCoroutine(CanRecovery());
        }

        EventManager<float>.Instance.TriggerEvent("UpdateCorruptionBar", Corruption);
    }

    private IEnumerator CorruptionCoroutine()
    {
        yield return new WaitForSeconds(invincibilitySeconds);
        invincibility = false;
        yield return new WaitForSeconds(fullyCorruptionTime);
        corrupted = false;

        yield return new WaitForSeconds(.2f);
        canRecover = true;
        isRecovering = true;
        elapsedTime = 0;

        EventManager<float>.Instance.TriggerEvent("UpdateCorruptionBar", Corruption);
    }

    private IEnumerator CanRecovery()
    {
        yield return new WaitForSeconds(recoverCorruptionWaitTime);
        canRecover = true;
        elapsedTime = 0;
    }

    private float EaseInCubic(float t)
    {
        return t * t * t;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Collectible>())
        {
            if (!isRecovering)
            {
                canRecover = false;
            }
        }
    }
    //private void OnTriggerStay2D(Collision2D collision)
    //{
    //    if (collision.transform.GetComponent<IEnemy>() != null && corrupted && !invincibility)
    //    {
    //        EventManager<GameState>.Instance.TriggerEvent("onPlayerDead", GameState.LOSING);
    //    }
    //}
    #endregion


    #region Dash

    [Header("Dash")][SerializeField]
    private float _dashSpeed = 15;
    [SerializeField]
    private float dashLength = 1;
    [SerializeField] private GameObject DashEffect;

    [SerializeField] private bool isDashing;
    [SerializeField] private bool canDash = true;
    [SerializeField] private float dashTime;
    [SerializeField] private float dashCooldown;
    [SerializeField] private Vector2 dashDir;

    private void Dash(bool _isDashing)
    {
        if (canDash && !corrupted)
        {
            canDash = false;
            isDashing = _isDashing;
            dashDir = new Vector2(direction.x, direction.y).normalized;
            if (dashDir == Vector2.zero) dashDir = isFacingRight ? Vector3.right : Vector3.left;
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
            _fallSpeed = 0;
            //GameObject dashEffect = Instantiate(DashEffect, transform.position + Vector3.up, DashEffect.transform.rotation);
            //float dashDirection = isFacingRight ? Vector2.right.x : Vector2.left.x;
            //Vector3 localScale = dashEffect.transform.localScale;
            //localScale.z = dashDirection;
            //dashEffect.transform.localScale = localScale;

        }
    }

    private void HandleDashing()
    {
        if (isDashing)
        {
            velocity = dashDir * _dashSpeed;
            if (velocity.x > 0 && _isAgainstRightWall || velocity.x < 0 && _isAgainstLeftWall)
            {
                isDashing = false;
                canDash = true;
                rb.gravityScale = 1;
                StartCoroutine(DashingCooldown());
            }

               StartCoroutine(StopDashing());
            
        }
    }

    private IEnumerator StopDashing()
    {
        yield return new WaitForSeconds(dashTime);
        isDashing = false;
        rb.gravityScale = 1;
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    private IEnumerator DashingCooldown()
    {
        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }

    #endregion
}

