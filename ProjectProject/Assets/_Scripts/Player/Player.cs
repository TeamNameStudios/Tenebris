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

    public TentaclePlatform tentacle;
    private bool _canCreateTentacle;
    private float initPose;
    private float endPose;

    private bool dashing;
    public AnimationCurve dashCurve;
    public float dashTimer;
    private float _dTimer;
    public float dashDistance;

    public TentacleAttack tentAtac;
    private bool _canAtac;
    private float intPose;
    private float finPose;
    private bool hooked;
    private Vector2 inPose;
    private Vector2 fnPose;
    public AnimationCurve hookAnim;
    private float hooTimer;
    private float hookCurrentTimer;
    public float postHookVel;
    public float postHookDecel = 9 / 10;



    private void Awake()
    {
     
    }

    private void OnEnable()
    {
        EventManager<Vector2>.Instance.StartListening("leftMovement",Move);
        EventManager<Vector2>.Instance.StartListening("rightMovement", Move);
        EventManager<bool>.Instance.StartListening("jumpMovement", Jump);
        EventManager<bool>.Instance.StartListening("createTentacle", TentacleUp);
        EventManager<bool>.Instance.StartListening("finishTentacle", TentacleFinish);
        EventManager<bool>.Instance.StartListening("dashMovement", Dash);
        EventManager<bool>.Instance.StartListening("attac", Attack);
        EventManager<Vector2>.Instance.StartListening("hook", Hook);
    }
    private void OnDisable()
    {
        EventManager<Vector2>.Instance.StopListening("leftMovement", Move);
        EventManager<Vector2>.Instance.StopListening("rightMovement", Move);
        EventManager<bool>.Instance.StopListening("jumpMovement", Jump);
        EventManager<bool>.Instance.StopListening("createTentacle", TentacleUp);
        EventManager<bool>.Instance.StopListening("finishTentacle", TentacleFinish);
        EventManager<bool>.Instance.StopListening("dashMovement", Dash);
        EventManager<bool>.Instance.StopListening("attac", Attack);
        EventManager<Vector2>.Instance.StopListening("hook", Hook);
    }

    private void Move(Vector2 movementDirection)
    {
        velocity.x = movementDirection.x * speed;
        EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", velocity.x);
    }

    private void TentacleUp(bool sentinel)
    {
        if (_canCreateTentacle && sentinel && isGrounded)
        {
            _canCreateTentacle = false;
            tentacle.TentacleInit(transform.position.y - 6);
            endPose = transform.position.y + tentacle.tentacleLenght;
            initPose = transform.position.y;
        }
    }
    private void TentacleFinish(bool canCreateAgain)
    {
        _canCreateTentacle = canCreateAgain;
    }

    private void Dash(bool isDashing)
    {
        dashing = isDashing;

        if(dashing)
        {
            _dTimer = 0;
            endPose = transform.position.x + dashDistance;
            initPose = transform.position.x;
            EventManager<Vector2>.Instance.StopListening("leftMovement", Move);
            EventManager<Vector2>.Instance.StopListening("rightMovement", Move);
            EventManager<bool>.Instance.StopListening("jumpMovement", Jump);
        }
    }

    private void Attack(bool sentinel)
    {
        if (_canCreateTentacle && sentinel && isGrounded)
        {
            _canAtac = false;
            tentAtac.TentacleInit(transform.position);
        }
    }
    private void Jump(bool isJumping)
    {
        if (isGrounded && isJumping)
        {
            isGrounded = false;
            velocity.y = jumpVelocity;
        }
    }

    private void Hook(Vector2 hitCollision)
    {
        hooked = true;

        inPose = transform.position;
        fnPose = hitCollision;

        hooTimer = tentAtac.currentTimer;
        hookCurrentTimer = 0;  
    }

    private void FixedUpdate()
    {
        Vector2 pos = transform.position;
        if (!_canCreateTentacle && isGrounded)
        {
            float t = tentacle.animCurve.Evaluate(tentacle.currentTimer + Time.fixedDeltaTime);
            float lerp = Mathf.Lerp(initPose, endPose, t);

            Vector2 posY = transform.position;
            posY.y = lerp;
            pos = posY;
        }
        if (dashing)
        {
            _dTimer += Time.fixedDeltaTime;
            if (_dTimer < dashTimer)
            {
                float t = dashCurve.Evaluate(_dTimer);
                float lerp = Mathf.Lerp(0, dashDistance, t);
                lerp = velocity.x >= 0 ? lerp : -lerp;
                EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", lerp);

            }
            else
            {
                dashing = false;
                EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", velocity.x);
                EventManager<Vector2>.Instance.StartListening("leftMovement", Move);
                EventManager<Vector2>.Instance.StartListening("rightMovement", Move);
                EventManager<bool>.Instance.StartListening("jumpMovement", Jump);
            }
        }
        else if(hooked)
        {
            hookCurrentTimer += Time.fixedDeltaTime;

            if (hookCurrentTimer < hooTimer)
            {
                velocity.y = 0;
                float t = hookAnim.Evaluate(hookCurrentTimer/hooTimer);
                float lerp = Mathf.Lerp(inPose.x, fnPose.x, t);

                Vector2 posX = transform.position;
                float posDelta = lerp - posX.x;
                posX.x = lerp;

                pos = posX;
            }
            else
            {
                velocity.x = postHookVel;
                EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", velocity.x);
                hooked = false;
                tentAtac.hook = false;
            }
        }
        else if(hooTimer > 0)
        {
            hooTimer -= Time.fixedDeltaTime;
            velocity.x *= postHookDecel;
        }
        else if (!isGrounded)
        {
            pos.y += velocity.y * Time.fixedDeltaTime;
            velocity.y += gravity * Time.fixedDeltaTime;

            Vector2 raycastOrigin = new Vector2((pos.x + 0.7f) * direction.x, pos.y);
            Vector2 rayDirection = Vector2.down;
            float rayDistance = Math.Abs(velocity.y) * Time.fixedDeltaTime;
            RaycastHit2D hit2D = Physics2D.Raycast(raycastOrigin, rayDirection, rayDistance);
            Ray ee = new Ray(raycastOrigin, rayDirection);
            Physics.Raycast(ee, out RaycastHit hit3D, rayDistance);
            if (hit2D.collider != null)
            {
                Terrain terrain = hit2D.collider.GetComponent<Terrain>();
                if (terrain != null)
                {
                    groundHeight = terrain.terrainHeight;
                    pos.y = groundHeight;
                    velocity.y = 0f;
                    isGrounded = true;
                }
            }
            else if(hit3D.collider != null)
            {
                hit3D.collider.CompareTag("Pushable");
                groundHeight = transform.position.y;
                pos.y = groundHeight;
                velocity.y = 0f;
                isGrounded = true;
            }
            Debug.DrawRay(raycastOrigin, rayDirection * rayDistance, Color.cyan);
        }
        else if (isGrounded)
        {
            Vector2 raycastOrigin = new Vector2((pos.x - 0.7f) * direction.x, pos.y);
            Vector2 rayDirection = Vector2.down;
            float rayDistance = Math.Abs(MathF.Max(velocity.y, 50f)) * Time.fixedDeltaTime;
            RaycastHit2D hit2D = Physics2D.Raycast(raycastOrigin, rayDirection, rayDistance);
            Ray ee = new Ray(raycastOrigin, rayDirection);
            Physics.Raycast(ee, out RaycastHit hit3D, rayDistance);
            if (hit2D.collider == null && hit3D.collider == null)
            {
                isGrounded = false;
            }
            Debug.DrawRay(raycastOrigin, rayDirection * rayDistance, Color.green);
        }
        transform.position = pos;
    }
}
