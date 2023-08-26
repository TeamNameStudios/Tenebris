using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Player : MonoBehaviour
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
        GetHookableObject();
        HandleGrapple();
        if (!isGrappling)
        {
            HandleDashing();
        }
        if (!isDashing && !isGrappling)
        {
            CalculateJump();
            CalculateMovement();
        }
        PerformMovement();
        ManageCorruption();


        if (IsGrounded && !isDashing && !isGrappling && velocity.x != 0)
        {
            // play clip walking on grass
            EventManager<bool>.Instance.TriggerEvent("onRunning", true);
        }
        else
        {
            EventManager<bool>.Instance.TriggerEvent("onRunning", false);
        }
        
        EventManager<bool>.Instance.TriggerEvent("onFullyCorrupted", corrupted);

        if (corrupted)
        {
            if (!corruptionPS.isPlaying)
            {
                corruptionPS.Play();
            }
        }
        else
        {
            if (corruptionPS.isPlaying)
            {
                corruptionPS.Stop();
            }
        }
        if(transform.position.y < -20)
        {
            EventManager<GameState>.Instance.TriggerEvent("onPlayerDead", GameState.LOSING);
        }
       
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (!landingPS.isPlaying)
            {
                landingPS.Play();
            }
        }
    }

    private void OnEnable()
    {
        EventManager<Vector2>.Instance.StartListening("movement", Move);
        EventManager<bool>.Instance.StartListening("jumpMovement", Jump);
        EventManager<float>.Instance.StartListening("Corruption", AddCorruption);
        EventManager<float>.Instance.StartListening("onCollectiblePickup", DecreaseCorruption);
        EventManager<bool>.Instance.StartListening("isDashing", Dash);
        EventManager<bool>.Instance.StartListening("isGrappling", Grappling);
        EventManager<List<PowerUp>>.Instance.StartListening("onPowerUpLoaded", PowerUpManager);
    }
    private void OnDisable()
    {
        EventManager<Vector2>.Instance.StopListening("movement", Move);
        EventManager<bool>.Instance.StopListening("jumpMovement", Jump);
        EventManager<float>.Instance.StopListening("Corruption", AddCorruption);
        EventManager<float>.Instance.StopListening("onCollectiblePickup", DecreaseCorruption);
        EventManager<bool>.Instance.StopListening("isDashing", Dash);
        EventManager<bool>.Instance.StopListening("isGrappling", Grappling);
        EventManager<List<PowerUp>>.Instance.StopListening("onPowerUpLoaded", PowerUpManager);
    }

    
    #region Detection

    [Header("Detection")]
    [SerializeField]
    private LayerMask _groundMask;
    [SerializeField]
    private Bounds _characterBounds;
    [SerializeField]
    private float _grounderRadius = 0.2f;
    [SerializeField]
    private float groundOffset = 0.2f;
    [SerializeField]
    private float _wallCheckRadius = 0.05f;
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
        bool grounded1 = Physics2D.OverlapCircleNonAlloc(transform.position + new Vector3(groundOffset, _characterBounds.min.y), _grounderRadius, _ground, _groundMask) > 0;
        bool grounded2 = Physics2D.OverlapCircleNonAlloc(transform.position + new Vector3(0, _characterBounds.min.y), _grounderRadius, _ground, _groundMask) > 0;
        bool grounded3 = Physics2D.OverlapCircleNonAlloc(transform.position + new Vector3(-groundOffset, _characterBounds.min.y), _grounderRadius, _ground, _groundMask) > 0;

        bool grounded = grounded1 || grounded2 || grounded3;
        if (!IsGrounded && grounded)
        {
            IsGrounded = true;
            isLaunchedState = false;
            _coyoteUsable = true;
        }
        else if (IsGrounded && !grounded)
        {
            _timeLeftGrounded = Time.time;
            IsGrounded = false;
        }

        bool _isAgainstRoof1 = Physics2D.OverlapCircleNonAlloc(transform.position + new Vector3(groundOffset, _characterBounds.max.y), _grounderRadius, _upWall, _groundMask) > 0;
        bool _isAgainstRoof2 = Physics2D.OverlapCircleNonAlloc(transform.position + new Vector3(0, _characterBounds.max.y), _grounderRadius, _upWall, _groundMask) > 0;
        bool _isAgainstRoof3 = Physics2D.OverlapCircleNonAlloc(transform.position + new Vector3(-groundOffset, _characterBounds.max.y), _grounderRadius, _upWall, _groundMask) > 0;
        _isAgainstRoof = _isAgainstRoof1 || _isAgainstRoof2 || _isAgainstRoof3;

        bool _isAgainstLeftWall1 = Physics2D.OverlapCircleNonAlloc(transform.position + new Vector3(_characterBounds.min.x, _characterBounds.max.y / 2), _wallCheckRadius, _leftWall, _groundMask) > 0;
        bool _isAgainstLeftWall2 = Physics2D.OverlapCircleNonAlloc(transform.position + new Vector3(_characterBounds.min.x, _characterBounds.center.y), _wallCheckRadius, _leftWall, _groundMask) > 0;
        bool _isAgainstLeftWall3 = Physics2D.OverlapCircleNonAlloc(transform.position + new Vector3(_characterBounds.min.x, _characterBounds.min.y / 2), _wallCheckRadius, _leftWall, _groundMask) > 0;
        bool _isAgainstRightWall1 = Physics2D.OverlapCircleNonAlloc(transform.position + new Vector3(_characterBounds.max.x, _characterBounds.max.y / 2), _wallCheckRadius, _rightWall, _groundMask) > 0;
        bool _isAgainstRightWall2 = Physics2D.OverlapCircleNonAlloc(transform.position + new Vector3(_characterBounds.max.x, _characterBounds.center.y), _wallCheckRadius, _rightWall, _groundMask) > 0;
        bool _isAgainstRightWall3 = Physics2D.OverlapCircleNonAlloc(transform.position + new Vector3(_characterBounds.max.x, _characterBounds.min.y / 2), _wallCheckRadius, _rightWall, _groundMask) > 0;
        _isAgainstLeftWall = _isAgainstLeftWall1 || _isAgainstLeftWall2 || _isAgainstLeftWall3;
        _isAgainstRightWall = _isAgainstRightWall1 || _isAgainstRightWall2 || _isAgainstRightWall3;
        if (isSmokeWallHit)
        {
            _isAgainstLeftWall = false;
        }

    }


    private void DrawGrounderGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(groundOffset, _characterBounds.max.y), _grounderRadius);
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, _characterBounds.max.y), _grounderRadius);
        Gizmos.DrawWireSphere(transform.position + new Vector3(-groundOffset, _characterBounds.max.y), _grounderRadius);


        Gizmos.DrawWireSphere(transform.position + new Vector3(groundOffset, _characterBounds.min.y), _grounderRadius);
        Gizmos.DrawWireSphere(transform.position + new Vector3(0, _characterBounds.min.y), _grounderRadius);
        Gizmos.DrawWireSphere(transform.position + new Vector3(-groundOffset, _characterBounds.min.y), _grounderRadius);
    }

    private void OnDrawGizmos()
    {
        DrawGrounderGizmos();
        DrawWallGizmos();
        DrawHookableZoneGizmos();
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + _characterBounds.center, _characterBounds.size);
    }

    private void DrawWallGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(_characterBounds.max.x, _characterBounds.max.y / 2), _wallCheckRadius);
        Gizmos.DrawWireSphere(transform.position + new Vector3(_characterBounds.max.x, _characterBounds.center.y), _wallCheckRadius);
        Gizmos.DrawWireSphere(transform.position + new Vector3(_characterBounds.max.x, _characterBounds.min.y / 2), _wallCheckRadius);

        Gizmos.DrawWireSphere(transform.position + new Vector3(_characterBounds.min.x, _characterBounds.max.y / 2), _wallCheckRadius);
        Gizmos.DrawWireSphere(transform.position + new Vector3(_characterBounds.min.x, _characterBounds.center.y), _wallCheckRadius);
        Gizmos.DrawWireSphere(transform.position + new Vector3(_characterBounds.min.x, _characterBounds.min.y / 2), _wallCheckRadius);


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
        if (!isDashing && !isGrappling && isFacingRight && velocity.x < 0f || !isFacingRight && velocity.x > 0f)
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
            
            EventManager<AudioClip>.Instance.TriggerEvent("onPlayJumpClip", JumpingClip);

            if (landingPS.isPlaying)
            {
                landingPS.Stop();
            }



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

    #region Corruption
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
        if (!corrupted && canRecover && GameController.Instance.State == GameState.PLAYING)
        {
            if (Corruption > 0)
            {
                isRecovering = true;
                float normalizedTime = (elapsedTime / maxCorruption) + recoveryStartingSmoothness;
                float easeTime = EaseInCubic(normalizedTime);
                DecreaseCorruption(recoverCorruptionSpeed * easeTime);
                elapsedTime += Time.deltaTime;
                //Debug.Log("Recovering " + normalizedTime * recoverCorruptionSpeed + " corruption");
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
            
            if (Corruption >= maxCorruption && !corrupted)
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

        //yield return new WaitForSeconds(.2f);
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


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.transform.GetComponent<IEnemy>() != null && corrupted && !invincibility)
        {
            EventManager<GameState>.Instance.TriggerEvent("onPlayerDead", GameState.LOSING);
            Debug.Log("DEAD");
        }
    }
    #endregion

    #region Dash

    [Header("DASH")]
    [SerializeField]
    private float dashSpeed = 15;
    [SerializeField] private bool isDashing;
    [SerializeField] private bool canDash = true;
    [SerializeField] private float startingDashTime;
    [SerializeField] private float startingDashCooldown;// value added only when the dash starts
    [SerializeField] private float dashCorruption; 

    private float dashTime;
    private float dashCooldown;
    private Vector2 dashDir;
    private bool isSmokeWallHit;

    private void Dash(bool _isDashing)
    {
        if (canDash)
        {
            EventManager<AudioClip>.Instance.TriggerEvent("onPlayClip", DashingClip);

            canDash = false;
            isDashing = _isDashing;
            dashTime = !corrupted ? startingDashTime : startingDashTime*2;
            dashCooldown = startingDashCooldown;
            EventManager<float>.Instance.TriggerEvent("Corruption", dashCorruption);
            dashDir = new Vector2(direction.x, 0);
            if (dashDir == Vector2.zero) dashDir = isFacingRight ? Vector3.right : Vector3.left;
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
            _fallSpeed = 0;
            DashEffect.Play();

        }
    }

    private void HandleDashing()
    {
        if (isDashing)
        {
            velocity = !corrupted ? dashDir * dashSpeed  : dashDir * dashSpeed / 2; 
            if (velocity.x > 0 && _isAgainstRightWall || velocity.x < 0 && _isAgainstLeftWall || velocity.y > 0 && _isAgainstRoof || velocity.y < 0 && IsGrounded)
            {
                isDashing = false;
                EventManager<bool>.Instance.TriggerEvent("isFinishDashed", true);
                EventManager<float>.Instance.TriggerEvent("Corruption", 0);
                rb.gravityScale = 1;
                dashTime = 0;
                DashEffect.Stop();
            }
            if (dashTime >= 0)
            {
                dashTime -= Time.deltaTime;
            }
            else
            {
                isDashing = false;
                EventManager<bool>.Instance.TriggerEvent("isFinishDashed", true);
                EventManager<float>.Instance.TriggerEvent("Corruption", 0);
                DashEffect.Stop();
                rb.gravityScale = 1;
            }
        }
        else
        {
            if(canDash == false)
            {
                if (dashCooldown >= 0)
                {
                    dashCooldown -= Time.deltaTime;
                }
                else
                {
                    canDash = true;
                }
            }

        }
    }

    //private IEnumerator StopDashing()
    //{
    //    yield return new WaitForSeconds(dashTime);
    //    DashEffect.Stop();
    //    isDashing = false;
    //    rb.gravityScale = 1;
    //    yield return new WaitForSeconds(dashCooldown);
    //    canDash = true;
    //}
    //private IEnumerator DashingCooldown()
    //{
    //    yield return new WaitForSeconds(dashCooldown);
    //    canDash = true;
    //}

    #endregion

    #region Grapple
    [Header("GRAPPLE")]
    [SerializeField]
    private bool isGrappling;
    [SerializeField]
    private bool isLaunchedState;
    [SerializeField]
    private bool canGrapple = true;
    [SerializeField]
    private Vector2 grappleDirection;
    [SerializeField]
    private Vector2 launchedStateDirection;
    [SerializeField]
    private float launchedSpeed;
    [SerializeField]
    private float grappleSpeed;
    [SerializeField]
    private HookableObject HookableObject;
    [SerializeField]
    private LayerMask hookableLayer;
    [SerializeField]
    private Vector2 swingingDirection;
    [SerializeField]
    private float chargeGrappleTime;

    [SerializeField]
    private float grappleZoneDistance;

    [Tooltip("Value to add only once if the corruptionOverTime box is CHECKED")]
    [SerializeField] private float hookCorruptionOverTime;
    [Tooltip("Value to add only once if the corruptionOverTime box is UNCHECKED")]
    [SerializeField] private float hookCorruptionOnce;
    [Tooltip("Check this box if the corruption should grow as long as the player is holding the grapple button\nUncheck this box if the corruption should grow only once")]
    [SerializeField] private bool corruptionOverTime;

    private void GetHookableObject()
    {
        Vector2 pos = transform.position;
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        Collider2D[] HookableHits = Physics2D.OverlapCircleAll(pos + direction, grappleZoneDistance);
        HookableObject = GetNearestHookable(HookableHits);

    }

    public HookableObject GetNearestHookable(Collider2D[] hookableHits)
    {
        Vector2 pos = transform.position;
        HookableObject nearestHookableObject = null;
        float minDist = Mathf.Infinity;
        for (int i = 0; i < hookableHits.Length; i++)
        {
            HookableObject hookableObject;
            if (hookableHits[i].TryGetComponent<HookableObject>(out hookableObject))
            {
                float dist = Vector3.Distance(hookableObject.transform.position, pos);
                if (dist < minDist)
                {
                    nearestHookableObject = hookableObject;
                    minDist = dist;
                }
            }
        }
        return nearestHookableObject;
    }

    private void DrawHookableZoneGizmos()
    {
        Gizmos.color = Color.red;
        Vector2 pos = transform.position;
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        Gizmos.DrawWireSphere(pos + direction, grappleZoneDistance);
        if (HookableObject != null)
        {
            Vector2 hookObjectpos = HookableObject.transform.position;
            Gizmos.DrawLine(pos + new Vector2(_characterBounds.max.x * direction.x, _characterBounds.max.y), hookObjectpos);
            Gizmos.DrawLine(pos + new Vector2(_characterBounds.max.x * direction.x, _characterBounds.center.y), hookObjectpos);
            Gizmos.DrawLine(pos + new Vector2(_characterBounds.max.x * direction.x, _characterBounds.min.y), hookObjectpos);
        }
    }

    private void Grappling(bool _isGrappling)
    {
        if (canGrapple && HookableObject != null && !isGrappling)
        {

            Vector2 pos = transform.position;
            Vector2 hookObjectpos = HookableObject.transform.position;
            swingingDirection = isFacingRight ? Vector2.right : Vector2.left;
            if ((swingingDirection.x > 0 && hookObjectpos.x < pos.x) || (swingingDirection.x < 0 && hookObjectpos.x > pos.x) /*|| hookObjectpos.y < pos.y*/)
            {
                return;
            }
            //RaycastHit2D hit = Physics2D.Raycast(pos + new Vector2(_characterBounds.max.x * direction.x, _characterBounds.max.y), hookObjectpos);
            //RaycastHit2D hitTop = Physics2D.Raycast(pos + new Vector2(_characterBounds.max.x * direction.x, _characterBounds.center.y), hookObjectpos);
            //RaycastHit2D hitBottom = Physics2D.Raycast(pos + new Vector2(_characterBounds.max.x * direction.x, _characterBounds.min.y), hookObjectpos);
            //if (hit != null && !hit.collider.GetComponent<HookableObject>() && !hitTop.collider.GetComponent<HookableObject>() && !hitBottom.collider.GetComponent<HookableObject>())
            //{
            //    return;
            //}

            isGrappling = _isGrappling;
            EventManager<AudioClip>.Instance.TriggerEvent("onPlayClip", GrapplingClip);
            if (!corruptionOverTime && !isLaunchedState)
            {
                EventManager<float>.Instance.TriggerEvent("Corruption", hookCorruptionOnce);

            }
            rb.gravityScale = 0;
            canGrapple = false;
            StartCoroutine(ChargingGrapple(hookObjectpos, pos));
        }
        else if (isGrappling)
        {
            rb.gravityScale = 1;
            isGrappling = false;
            canGrapple = true;
        }
    }

    private void HandleGrapple()
    {
        if (HookableObject != null)
        {
            Vector2 pos = transform.position;
            Vector2 hookObjectpos = HookableObject.transform.position;
            if (isGrappling)
            {
                
                velocity = grappleDirection * grappleSpeed;
                if (corruptionOverTime && !isLaunchedState)
                {
                    EventManager<float>.Instance.TriggerEvent("Corruption", hookCorruptionOverTime);
                }
                if ((grappleDirection.x > 0 && hookObjectpos.x < pos.x) || (grappleDirection.x < 0 && hookObjectpos.x > pos.x))
                {
                    isLaunchedState = true;
                    isGrappling = false;
                    canGrapple = true;
                    grappleDirection = Vector2.zero;
                    rb.gravityScale = 1;
                    velocity = new Vector2(launchedStateDirection.x * swingingDirection.x, launchedStateDirection.y) * launchedSpeed;
                }
            }
        }

    }

    private IEnumerator ChargingGrapple(Vector2 hookObjectpos, Vector2 pos)
    {
        if (isLaunchedState)
        {
            rb.velocity = Vector2.zero;
            velocity = Vector2.zero;
            grappleDirection = (hookObjectpos - pos).normalized;
            yield return new WaitForSeconds(0);
        }
        else
        {
            rb.velocity = Vector2.zero;
            velocity = Vector2.zero;
            yield return new WaitForSeconds(chargeGrappleTime);
            grappleDirection = (hookObjectpos - pos).normalized;

        }

    }
    #endregion


    #region PowerUpManager 

    private void PowerUpManager(List<PowerUp> powerUps)
    {
        for(int i = 0; i < powerUps.Count; i++)
        {
            switch(powerUps[i].ID) {
                case PowerUpEnum.DASH_SPEED:
                    {
                        ScriptablePowerUp scriptablePowerUp = ResourceSystem.Instance.GetPowerUp(powerUps[i].ID, powerUps[i].Level);
                        dashSpeed = dashSpeed + (dashSpeed * scriptablePowerUp.PowerUpPercentage / 100);
                    }
                    break;
                case PowerUpEnum.DASH_COOLDOWN:
                    {
                        ScriptablePowerUp scriptablePowerUp = ResourceSystem.Instance.GetPowerUp(powerUps[i].ID, powerUps[i].Level);
                        startingDashCooldown = startingDashCooldown - (startingDashCooldown * scriptablePowerUp.PowerUpPercentage / 100);
                    }
                    break;
                case PowerUpEnum.DASH_CORRUPTION_USAGE:
                    {
                        ScriptablePowerUp scriptablePowerUp = ResourceSystem.Instance.GetPowerUp(powerUps[i].ID, powerUps[i].Level);
                        dashCorruption = dashCorruption - (dashCorruption * scriptablePowerUp.PowerUpPercentage / 100);
                    }
                    break;
                case PowerUpEnum.GRAPPLE_SPEED:
                    {
                        ScriptablePowerUp scriptablePowerUp = ResourceSystem.Instance.GetPowerUp(powerUps[i].ID, powerUps[i].Level);
                        grappleSpeed = grappleSpeed + (grappleSpeed * scriptablePowerUp.PowerUpPercentage / 100);
                    }
                    break;
                case PowerUpEnum.GRAPPLE_LAUNCH_SPEED:
                    {
                        ScriptablePowerUp scriptablePowerUp = ResourceSystem.Instance.GetPowerUp(powerUps[i].ID, powerUps[i].Level);
                        launchedSpeed = launchedSpeed + (launchedSpeed * scriptablePowerUp.PowerUpPercentage / 100);
                    }
                    break;
                case PowerUpEnum.GRAPPLE_CORRUPTION_USAGE:
                    {
                        ScriptablePowerUp scriptablePowerUp = ResourceSystem.Instance.GetPowerUp(powerUps[i].ID, powerUps[i].Level);
                        hookCorruptionOnce = hookCorruptionOnce - (hookCorruptionOnce * scriptablePowerUp.PowerUpPercentage / 100);
                    }
                    break;
            }
        }
    }

    #endregion

    #region Audio
    [Header("AUDIO CLIPS")]
    [SerializeField] private AudioClip DashingClip;
    [SerializeField] private AudioClip JumpingClip;
    [SerializeField] private AudioClip GrapplingClip;
    #endregion

    #region ParticleSystem
    [Header("PARTICLE EFFECTS")]
    [SerializeField]
    private ParticleSystem DashEffect;
    [SerializeField]
    private ParticleSystem corruptionPS;
    [SerializeField]
    private ParticleSystem landingPS;
    [SerializeField]
    private ParticleSystem DashCooldownPS;
    #endregion

}

