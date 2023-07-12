using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class Player : MonoBehaviour
{

    [SerializeField]
    public Vector2 velocity;
    [SerializeField]
    public Vector2 direction = Vector2.zero;
    //[SerializeField]
    //public bool isGrounded = false;
    [SerializeField]
    public bool isDashing = false;

    public bool JumpingThisFrame;
    public bool LandingThisFrame;

    [SerializeField]
    private PlayerJump playerJump;

    [SerializeField]
    private PlayerMovement playerMovement;
    private void Awake()
    {
        playerJump = GetComponent<PlayerJump>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        //if (!_active) return;
        // Calculate velocity
        //velocity = (transform.position - _lastPosition) / Time.deltaTime;
        //_lastPosition = transform.position;

        //GatherInput();
        RunCollisionChecks();

        playerMovement.CalculateWalk(); // Horizontal movement
        playerJump.CalculateJumpApex(); // Affects fall speed, so calculate before gravity
        CalculateGravity(); // Vertical movement
        playerJump.CalculateJump(); // Possibly overrides vertical

        MoveCharacter(); // Actually perform the axis movement
    }

    [Header("COLLISION")][SerializeField] private Bounds _characterBounds;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private int _detectorCount = 3;
    [SerializeField] private float _detectionRayLength = 0.1f;
    [SerializeField][Range(0.1f, 0.3f)] private float _rayBuffer = 0.1f; // Prevents side detectors hitting the ground

    private RayRange _raysUp, _raysRight, _raysDown, _raysLeft;
    public bool _colUp, _colRight, _colDown, _colLeft;

    public float _timeLeftGrounded;

    // We use these raycast checks for pre-collision information
    private void RunCollisionChecks()
    {
        // Generate ray ranges. 
        CalculateRayRanged();

        // Ground
        LandingThisFrame = false;
        var groundedCheck = RunDetection(_raysDown);
        if (_colDown && !groundedCheck) _timeLeftGrounded = Time.time; // Only trigger when first leaving
        else if (!_colDown && groundedCheck)
        {
            playerJump._coyoteUsable = true; // Only trigger when first touching
            LandingThisFrame = true;
        }

        _colDown = groundedCheck;

        // The rest
        _colUp = RunDetection(_raysUp);
        _colLeft = RunDetection(_raysLeft);
        _colRight = RunDetection(_raysRight);

        bool RunDetection(RayRange range)
        {
            return EvaluateRayPositions(range).Any(point => Physics2D.Raycast(point, range.Dir, _detectionRayLength, _groundLayer));
        }
    }
    private void CalculateRayRanged()
    {
        // This is crying out for some kind of refactor. 
        var b = new Bounds(transform.position + _characterBounds.center, _characterBounds.size);

        _raysDown = new RayRange(b.min.x + _rayBuffer, b.min.y, b.max.x - _rayBuffer, b.min.y, Vector2.down);
        _raysUp = new RayRange(b.min.x + _rayBuffer, b.max.y, b.max.x - _rayBuffer, b.max.y, Vector2.up);
        _raysLeft = new RayRange(b.min.x, b.min.y + _rayBuffer, b.min.x, b.max.y - _rayBuffer, Vector2.left);
        _raysRight = new RayRange(b.max.x, b.min.y + _rayBuffer, b.max.x, b.max.y - _rayBuffer, Vector2.right);
    }

    private IEnumerable<Vector2> EvaluateRayPositions(RayRange range)
    {
        for (var i = 0; i < _detectorCount; i++)
        {
            var t = (float)i / (_detectorCount - 1);
            yield return Vector2.Lerp(range.Start, range.End, t);
        }
    }

    private void OnDrawGizmos()
    {
        // Bounds
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + _characterBounds.center, _characterBounds.size);

        // Rays
        //if (!Application.isPlaying)
        //{
        CalculateRayRanged();
        Gizmos.color = Color.blue;
        foreach (var range in new List<RayRange> { _raysUp, _raysRight, _raysDown, _raysLeft })
        {
            foreach (var point in EvaluateRayPositions(range))
            {
                Gizmos.DrawRay(point, range.Dir * _detectionRayLength);
            }
        }
        //}

        if (!Application.isPlaying) return;

        // Draw the future position. Handy for visualizing gravity
        Gizmos.color = Color.red;
        var move = new Vector3(velocity.x, velocity.y) * Time.deltaTime;
        Gizmos.DrawWireCube(transform.position + _characterBounds.center + move, _characterBounds.size);
    }

    #region Gravity

    [Header("GRAVITY")][SerializeField] private float _fallClamp = -40f;
    [SerializeField] public float _minFallSpeed = 80f;
    [SerializeField] public float _maxFallSpeed = 120f;
    public float _fallSpeed;

    public void CalculateGravity()
    {
        if (_colDown)
        {
            // Move out of the ground
            if (velocity.y < 0) velocity.y = 0;
        }
        else
        {
            // Add downward force while ascending if we ended the jump early
            var fallSpeed = playerJump._endedJumpEarly && velocity.y > 0 ? _fallSpeed * playerJump._jumpEndEarlyGravityModifier : _fallSpeed;

            // Fall
            velocity.y -= fallSpeed * Time.deltaTime;

            // Clamp
            if (velocity.y < _fallClamp) velocity.y = _fallClamp;
        }
    }

    #endregion

    #region Move

    [Header("MOVE")]
    [SerializeField, Tooltip("Raising this value increases collision accuracy at the cost of performance.")]
    private int _freeColliderIterations = 10;

    // We cast our bounds before moving to avoid future collisions
    private void MoveCharacter()
    {
        Vector2 pos = transform.position + _characterBounds.center;
        Vector2 move = velocity * Time.deltaTime;
        Vector2 furthestPoint = pos + move;

        // check furthest movement. If nothing hit, move and don't do extra checks
        Collider2D hit = Physics2D.OverlapBox(furthestPoint, _characterBounds.size, 0, _groundLayer);
        if (!hit)
        {
            if (velocity.x > 0 && _colRight || velocity.x < 0 && _colLeft)
            {
                // Don't walk through walls
                velocity.x = 0;
            }
            EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", velocity.x);
            return;
        }

        // otherwise increment away from current pos; see what closest position we can move to
        //Vector2 realPosition = transform.position;
        //Vector2 position = transform.position;
        //Vector2 hitPos = hit.transform.position;
        //Vector2 positionToMoveTo = position;
     
        //for (int i = 1; i < _freeColliderIterations; i++)
        //{
        //    // increment to check all but furthestPoint - we did that already
        //    float t = (float)i / _freeColliderIterations;
        //    Vector2 posToTry = Vector2.Lerp(pos, furthestPoint, t);

        //    if (Physics2D.OverlapBox(posToTry, _characterBounds.size, 0, _groundLayer))
        //    {
        //        position = positionToMoveTo;

        //        // We've landed on a corner or hit our head on a ledge. Nudge the player gently
        //        if (i == 1)
        //        {
        //            if (velocity.y < 0) velocity.y = 0;
        //            Vector2 dir = position - hitPos;
        //            position += dir.normalized * move.magnitude;
        //            //Vector2 dir = position - hitPos;
        //            EventManager<Vector2>.Instance.TriggerEvent("moveWorld", dir);
        //        }

        //        return;
        //    }

        //    positionToMoveTo = posToTry;
        // }
        //Vector2 differenceToMove = realPosition - position;
        //EventManager<Vector2>.Instance.TriggerEvent("moveWorld", differenceToMove);
        #endregion
    }
}
public struct RayRange
{
    public RayRange(float x1, float y1, float x2, float y2, Vector2 dir)
    {
        Start = new Vector2(x1, y1);
        End = new Vector2(x2, y2);
        Dir = dir;
    }

    public readonly Vector2 Start, End, Dir;
}
