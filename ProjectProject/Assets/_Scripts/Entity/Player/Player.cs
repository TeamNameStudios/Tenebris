using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
        anim = GetComponent<Animator>();
        lineRenderer = GetComponent<LineRenderer>();
    }
    
    private void Update()
    {
        if (GameController.Instance.State == GameState.PLAYING || GameController.Instance.State == GameState.TUTORIAL)
        {
            RunCollisionChecks();
            CalculateJumpApex();
            CalculateGravity();
            if (!isGrappling)
            {
                GetHookableObject();
            }
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
            ManageAnimation();
            ManageParticle();
            ManageAudio();
            ManageDead();
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
        EventManager<Collectible>.Instance.StartListening("onCollectiblePickup", GetColletible);
        EventManager<bool>.Instance.StartListening("isDashing", Dash);
        EventManager<bool>.Instance.StartListening("isGrappling", Grappling);
        EventManager<List<PowerUp>>.Instance.StartListening("onPowerUpLoaded", PowerUpManager);
        EventManager<bool>.Instance.StartListening("onLevelUp", LevelUp);
    }
    private void OnDisable()
    {
        EventManager<Vector2>.Instance.StopListening("movement", Move);
        EventManager<bool>.Instance.StopListening("jumpMovement", Jump);
        EventManager<float>.Instance.StopListening("Corruption", AddCorruption);
        EventManager<Collectible>.Instance.StopListening("onCollectiblePickup", GetColletible);
        EventManager<bool>.Instance.StopListening("isDashing", Dash);
        EventManager<bool>.Instance.StopListening("isGrappling", Grappling);
        EventManager<List<PowerUp>>.Instance.StopListening("onPowerUpLoaded", PowerUpManager);
        EventManager<bool>.Instance.StopListening("onLevelUp", LevelUp);
    }

    private void ManageDead() {

        if (transform.position.y < -20)
        {
            if (GameController.Instance.IsTutorial == 1)
            {
                transform.position = new Vector2(0, 30);
            }
            else
            {
                Dead();
            }
        }
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
    public float maxVelocityIncrement;
    [SerializeField]
    public float acceleration;
    [SerializeField]
    public float accelerationIncrement;
    [SerializeField]
    public float deAcceleration;
    private void Move(Vector2 movementDirection)
    {
        direction = movementDirection;
        if(!isDashing && !isGrappling)
        {
            Flip(movementDirection);
        }
      
    }


    public void Flip(Vector2 direction)
    {
        if (isFacingRight && direction.x < 0f || !isFacingRight && direction.x > 0f)
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

    private void LevelUp(bool value)
    {
        maxVelocity += maxVelocityIncrement;
        acceleration += accelerationIncrement;
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

    [SerializeField] private float _jumpBuffer = 0.1f;
    private float _lastJumpPressed;
    private bool HasBufferedJump => IsGrounded && _lastJumpPressed + _jumpBuffer > Time.time;

    [SerializeField]
    private bool CanUseCoyote => _coyoteUsable && !IsGrounded && _timeLeftGrounded + _coyoteTimeThreshold > Time.unscaledTime;
    [SerializeField]
    private bool InputJump;
    private void Jump(bool _inputJump)
    {
        if (_inputJump)
        {
            _lastJumpPressed = Time.time;
        }

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
        if (HasBufferedJump || CanUseCoyote && InputJump)
        {
            InputJump = false;
            velocity.y = _jumpHeight;
            _coyoteUsable = false;
            _timeLeftGrounded = float.MinValue;
            EventManager<SoundEnum>.Instance.TriggerEvent("onPlayClip", SoundEnum.jumpSound);
            if (landingPS.isPlaying)
            {
                landingPS.Stop();
            }
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
        Vector3 velocityPos = velocity * Time.deltaTime;

        EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", velocity.x);
    }


    #endregion

    #region Corruption
    [Header("CORRUPTION_SYSTEM")]
    [SerializeField]
    private float Corruption;
    [SerializeField]
    private float maxCorruption;
    public float MaxCorruption { get { return maxCorruption; } }
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
        if (GameController.Instance != null)
        {
            if (!corrupted && canRecover && GameController.Instance.State == GameState.PLAYING)
            {
                if (Corruption > 0)
                {
                    isRecovering = true;
                    float normalizedTime = (elapsedTime / maxCorruption) + recoveryStartingSmoothness;
                    float easeTime = EaseInCubic(normalizedTime);
                    DecreaseCorruption(recoverCorruptionSpeed * easeTime);
                    elapsedTime += Time.unscaledDeltaTime;
                    //Debug.Log("Recovering " + normalizedTime * recoverCorruptionSpeed + " corruption");
                }
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

    private void  GetColletible (Collectible collectible)
    {
        DecreaseCorruption(collectible.CorruptionReductionValue);
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
        if (collision.transform.GetComponent<IEnemy>() != null && corrupted && !invincibility && GameController.Instance.IsTutorial == 0)
        {
            Dead();
        }
    }

    private void Dead()
    {
        EventManager<SoundEnum>.Instance.TriggerEvent("onPlayClip", SoundEnum.gameOverSound);
        EventManager<GameState>.Instance.TriggerEvent("onPlayerDead", GameState.LOSING);
    }
    #endregion

    #region Dash

    [Header("DASH")]
    [SerializeField]
    private float dashSpeed = 15;
    [SerializeField] private bool isDashing;
    [SerializeField] private bool canDash = true;
    [SerializeField] private float startingDashCooldown;// value added only when the dash starts
    //[Tooltip("Using dashRatio won't consider the startingDashTime and calculate the length of the dash independently")]
    [SerializeField] private float dashRatio;
    private float dashTimeRatio;
    [SerializeField] private float dashCorruptionRatio;
   private float dashCorruptionTimeRatio;
    [SerializeField] private float dashCorruption; 

    private float dashTime;
    private float dashCooldown;
    private Vector2 dashDir;
    private bool isSmokeWallHit;

    private void Dash(bool _isDashing)
    {

        if (canDash)
        {
            EventManager<SoundEnum>.Instance.TriggerEvent("onPlayClip", SoundEnum.dashSound);

            canDash = false;
            isDashing = _isDashing;
   
            dashTimeRatio = dashRatio / dashSpeed;
            dashCorruptionTimeRatio = dashCorruptionRatio / dashSpeed;
            dashTime = !corrupted ? dashTimeRatio : dashCorruptionTimeRatio;
    
           
            dashCooldown = startingDashCooldown;
            EventManager<float>.Instance.TriggerEvent("Corruption", dashCorruption);
            EventManager<bool>.Instance.TriggerEvent("onDash", true);
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
            velocity = !corrupted ? dashDir * dashSpeed  : dashDir * dashSpeed + new Vector2(-dashDir.x * 5,0); 
            if (velocity.x > 0 && _isAgainstRightWall || velocity.x < 0 && _isAgainstLeftWall || velocity.y > 0 && _isAgainstRoof || velocity.y < 0 && IsGrounded)
            {
                isDashing = false;
                EventManager<float>.Instance.TriggerEvent("Corruption", 0);
                EventManager<bool>.Instance.TriggerEvent("onDash", false);
                rb.gravityScale = 1;
                dashTime = 0;
                DashEffect.Stop();
            }
            if (dashTime > 0)
            {
                dashTime -= Time.deltaTime;
                if(dashTime <= 0)
                {
                    isDashing = false;
                    velocity.x = 0;
                    EventManager<bool>.Instance.TriggerEvent("onDash", false);
                    EventManager<float>.Instance.TriggerEvent("Corruption", 0);
                    DashEffect.Stop();
                    rb.gravityScale = 1;
                }
            }
        }
        else
        {
            if (canDash == false)
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

    [SerializeField]
    private LineRenderer lineRenderer;
    [SerializeField]
    Transform tentacleStartingPoint;
    private void GetHookableObject()
    {
        Vector2 pos = transform.position;
        Vector2 direction = isFacingRight ? Vector2.right : Vector2.left;
        Collider2D[] HookableHits = Physics2D.OverlapCircleAll(pos + direction, grappleZoneDistance);
        HookableObject _HookableObject = GetNearestHookable(HookableHits);
        if (HookableObject == null && _HookableObject != null)
        {
            HookableObject = _HookableObject;
            HookableObject.EnableGrapplable();
        }
        else if (_HookableObject == null && HookableObject != null)
        {
            HookableObject.DisableGrapplable();
            HookableObject = _HookableObject;
        }
        else if (_HookableObject != HookableObject)
        {
            HookableObject.DisableGrapplable();
            HookableObject = _HookableObject;
            HookableObject.EnableGrapplable();
        }

    }

    public HookableObject GetNearestHookable(Collider2D[] hookableHits)
    {
        Vector2 pos = transform.position;
        HookableObject nearestHookableObject = null;
        float minDist = Mathf.Infinity;
        Vector2 direction = isFacingRight? Vector2.right: Vector2.left;

        for (int i = 0; i < hookableHits.Length; i++)
        {

            HookableObject hookableObject;
            if (hookableHits[i].TryGetComponent<HookableObject>(out hookableObject))
            {
                Vector2 hookObjectPos = hookableObject.transform.position;
                float dist = Vector3.Distance(hookObjectPos, pos);

                if (dist < minDist && (direction.x > 0 && hookObjectPos.x > pos.x) || (direction.x < 0 && hookObjectPos.x < pos.x))
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


    [SerializeField]
    bool hitBottomGrapple;
    [SerializeField]
    bool hitTopGrapple;
    private void Grappling(bool _isGrappling)
    {
        hitBottomGrapple = false;
        hitTopGrapple = false;
        if (canGrapple && HookableObject != null && !isGrappling)
        {

            //hitGrapple = hitTopGrapple = hitBottomGrapple = false;
            Vector2 pos = transform.position;
            Vector2 hookObjectpos = HookableObject.transform.position;
            swingingDirection = isFacingRight ? Vector2.right : Vector2.left;
            if ((swingingDirection.x > 0 && hookObjectpos.x < pos.x) || (swingingDirection.x < 0 && hookObjectpos.x > pos.x) /*|| hookObjectpos.y < pos.y*/)
            {
                return;
            }

            Vector2 hitTopPoint = pos + new Vector2(_characterBounds.max.x * direction.x, _characterBounds.max.y);
            Vector2 hitBottomPoint = pos + new Vector2(_characterBounds.max.x * direction.x, _characterBounds.min.y + 1);
            float distanceTopHit = Vector2.Distance(hitTopPoint, hookObjectpos);
            float distanceBottomHit = Vector2.Distance(hitBottomPoint, hookObjectpos);
            RaycastHit2D[] hitTop = Physics2D.RaycastAll(hitTopPoint, hookObjectpos - hitTopPoint, distanceTopHit, _groundMask);
            RaycastHit2D[] hitBottom = Physics2D.RaycastAll(hitBottomPoint, hookObjectpos - hitBottomPoint   , distanceBottomHit, _groundMask);

            for (int i = 0; i < hitTop.Length; i++)
            {
                if (hitTop[i].collider != null)
                {
                    hitTopGrapple = true;
                }


            }
            for (int i = 0; i < hitBottom.Length; i++)
            {
                if (hitBottom[i].collider != null)
                {
                    hitBottomGrapple = true;
                }
            }
            if (hitTopGrapple || hitBottomGrapple)
            {
                return;
            }

            isGrappling = _isGrappling;
            EventManager<SoundEnum>.Instance.TriggerEvent("onPlayClip", SoundEnum.grappleSound);
            if (!corruptionOverTime && !isLaunchedState)
            {
                EventManager<float>.Instance.TriggerEvent("Corruption", hookCorruptionOnce);

            }
            rb.gravityScale = 0;
            canGrapple = false;
            lineRenderer.enabled = true;
            StartCoroutine(ChargingGrapple(hookObjectpos, pos));
        }
        else if (isGrappling)
        {
            rb.gravityScale = 1;
            isGrappling = false;
            canGrapple = true;
            lineRenderer.enabled = false;
        }
    }

    private void HandleGrapple()
    {
        if (HookableObject != null)
        {
            Vector2 pos = transform.position;
            Vector2 hookObjectpos = HookableObject.transform.position;
            lineRenderer.SetPosition(0, tentacleStartingPoint.position);
            lineRenderer.SetPosition(1, hookObjectpos);
            if (isGrappling)
            {
                
                velocity = grappleDirection * grappleSpeed;
                if (corruptionOverTime && !isLaunchedState)
                {
                    EventManager<float>.Instance.TriggerEvent("Corruption", hookCorruptionOverTime);
                }
                if ((grappleDirection.x > 0 && hookObjectpos.x < pos.x) || (grappleDirection.x < 0 && hookObjectpos.x > pos.x))
                {
                 
                        isGrappling = false;
                        canGrapple = true;
                        grappleDirection = Vector2.zero;
                        rb.gravityScale = 1;
                        lineRenderer.enabled = false;
                    if (!corrupted)
                    {
                        isLaunchedState = true;
                        velocity = new Vector2(launchedStateDirection.x * swingingDirection.x, launchedStateDirection.y) * launchedSpeed;
                    }

                }
            }
        }
        else
        {
            if (isGrappling)
            {
                lineRenderer.enabled = false;
               
                isGrappling = false;
                canGrapple = true;
                grappleDirection = Vector2.zero;
                rb.gravityScale = 1;
                if (!corrupted)
                {
                    isLaunchedState = true;
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
            ScriptablePowerUp scriptablePowerUp;
            int level = powerUps[i].Level -1;
            if (level >= 1)
            {
               scriptablePowerUp = ResourceSystem.Instance.GetPowerUp(powerUps[i].ID, level);
            }else {
                break;
            }
            switch (powerUps[i].ID) {
                case PowerUpEnum.DASH_SPEED:
                    {
                        if(level >= 1)
                        {
                            
                            dashSpeed = dashSpeed + (dashSpeed * scriptablePowerUp.PowerUpPercentage / 100);
                        }
                       
                    }
                    break;
                case PowerUpEnum.DASH_COOLDOWN:
                    {
                        if (powerUps[i].Level > 0)
                        {
                            
                            startingDashCooldown = startingDashCooldown - (startingDashCooldown * scriptablePowerUp.PowerUpPercentage / 100);
                        }
                    }
                    break;
                case PowerUpEnum.DASH_CORRUPTION_USAGE:
                    {
                        if (powerUps[i].Level > 0)
                        {
                         
                            dashCorruption = dashCorruption - (dashCorruption * scriptablePowerUp.PowerUpPercentage / 100);
                        }
                    }
                    break;
                case PowerUpEnum.GRAPPLE_SPEED:
                    {
                        if (powerUps[i].Level > 0)
                        {
                            
                            grappleSpeed = grappleSpeed + (grappleSpeed * scriptablePowerUp.PowerUpPercentage / 100);
                        }
                    }
                    break;
                case PowerUpEnum.GRAPPLE_LAUNCH_SPEED:
                    {
                        if (powerUps[i].Level > 0)
                        {
                           
                            launchedSpeed = launchedSpeed + (launchedSpeed * scriptablePowerUp.PowerUpPercentage / 100);
                        }
                    }
                    break;
                case PowerUpEnum.GRAPPLE_CORRUPTION_USAGE:
                    {
                        if (powerUps[i].Level > 0)
                        {

                            hookCorruptionOnce = hookCorruptionOnce - (hookCorruptionOnce * scriptablePowerUp.PowerUpPercentage / 100);
                        }
                    }
                    break;
            }
        }
    }

    #endregion

    #region Audio
    private void ManageAudio()
    {

        if (IsGrounded && !isDashing && !isGrappling && velocity.x != 0)
        {
            // play clip walking on grass
            EventManager<SoundEnum>.Instance.TriggerEvent("onPlayContinousClip", SoundEnum.runSound);
        }
        else
        {
            EventManager<SoundEnum>.Instance.TriggerEvent("onStopContinousClip", SoundEnum.runSound);
        }

        //EventManager<SoundEnum>.Instance.TriggerEvent("onPlayClip", SoundEnum.corruptionSound);
    }
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


    private void ManageParticle()
    {

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
    }
    #endregion

    #region Animation

    Animator anim;
    private static readonly int IdleAnimValue = Animator.StringToHash("Idle");
    private static readonly int WalkAnimValue = Animator.StringToHash("Walk");
    private static readonly int JumpAnimValue = Animator.StringToHash("Jump");
    private static readonly int FallAnimValue = Animator.StringToHash("Fall");
    private static readonly int LandAnimValue = Animator.StringToHash("Land");
    private static readonly int DashAnimValue = Animator.StringToHash("Dash");
    private static readonly int GrappleAnimValue = Animator.StringToHash("Grapple");
    private int currentAnimationState;
    private float lockedTill;
    private void ManageAnimation()
    {
        int state = GetAnimationState();
        if (state == currentAnimationState) return;
        anim.CrossFade(state, 0, 0);
        currentAnimationState = state;
    }

    private int GetAnimationState()
    {
        if (Time.time < lockedTill) return currentAnimationState;
        if (IsGrounded) return velocity.x == 0 ? IdleAnimValue : WalkAnimValue;
        return velocity.y > 0 ? JumpAnimValue : FallAnimValue;
    }

    private int LockState(int state,float lockTime)
    {
        lockedTill = Time.time + lockTime;
        return state;
    }
    #endregion

}

public struct OverlapCollidedResult {
    public bool isCollided;
    public float overlapValue;
}
