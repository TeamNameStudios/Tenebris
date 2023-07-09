using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHook : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;
    [SerializeField]
    private HookableObject HookableObject;
    [SerializeField]
    public bool isHooked;
    [SerializeField]
    private float minDistance;
    [SerializeField]
    private float actualDistance;
    [SerializeField]
    private float maxDistance;
    [SerializeField]
    private PlayerJump playerJump;
    [SerializeField]
    private PlayerMovement playerMovement;
    [SerializeField]
    private Player player;
    [SerializeField]
    float swingAccellerationSwing;
    [SerializeField]
    float maxSwingPower;
    [SerializeField]
    float distanceToCheck;
    [SerializeField]
    Vector2 jumpPowerAfterSwing;

    [SerializeField]
    private Vector2 swingingDirection;
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        playerJump = GetComponent<PlayerJump>();
        playerMovement = GetComponent<PlayerMovement>();
        player = GetComponent<Player>();
    }

    private void OnEnable()
    {
        EventManager<bool>.Instance.StartListening("hook", Hook);
    }
    private void OnDisable()
    {
        EventManager<bool>.Instance.StopListening("hook", Hook);
    }
    private void Update()
    {
        Vector2 pos = transform.position;
        if (HookableObject == null)
        {
            Collider2D[] HookableHits = Physics2D.OverlapCircleAll(pos, distanceToCheck);
            for (int i = 0; i < HookableHits.Length; i++)
            {
                HookableObject hookableObject;
                if (HookableHits[i].TryGetComponent<HookableObject>(out hookableObject))
                {
                    HookableObject = hookableObject;
                    return;
                }
            }

        }

    }


    //private void OnDrawGizmosSelected()
    //{
    //    Gizmos.DrawSphere(transform.position, distanceToCheck);
    //}

    private void FixedUpdate()
    {
        
        if (isHooked)
        {
            Vector2 pos = transform.position;
            Vector2 hookObjectpos = HookableObject.transform.position;
            lineRenderer.SetPosition(0, hookObjectpos);
            lineRenderer.SetPosition(1, pos);
            player.velocity.x += swingAccellerationSwing * swingingDirection.x * Time.fixedDeltaTime * 100f;
            player.velocity.x = Mathf.Clamp(player.velocity.x, -maxSwingPower, maxSwingPower);
            //pos.y = -Mathf.Sqrt(Mathf.Pow(minDistance, 2)- Mathf.Pow(hookObjectpos.x - pos.x, 2)) + hookObjectpos.y;
            pos.y = Mathf.Lerp(pos.y, hookObjectpos.y, Time.fixedDeltaTime * 2f);
            transform.position = Vector2.Lerp(transform.position, pos, Time.fixedDeltaTime * 100f);
            EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", player.velocity.x);
            if ((swingingDirection.x > 0 && hookObjectpos.x < pos.x) || (swingingDirection.x < 0 && hookObjectpos.x > pos.x))
            {
                isHooked = false;
                JumpAfterSwing();
            }

        }
    }

    private void Hook(bool _isHooked)
    {
        if(HookableObject != null)
        {
            float distance = Vector2.Distance(HookableObject.transform.position, transform.position);
            actualDistance = Mathf.Clamp(distance - 3f,minDistance,maxDistance);
            swingingDirection = playerMovement.isFacingRight ? Vector2.right : Vector2.left;

        }
        isHooked = _isHooked;
        if(_isHooked)
        {
            player.isGrounded = false;
            playerJump.gravity = 0f;
            player.velocity.y = 0f;
            lineRenderer.enabled = true;
        }
        else
        {
            lineRenderer.enabled = false;
            playerJump.gravity = playerJump.originalGravity;
        }
        //else
        //{
        //    JumpAfterSwing();
        //}


    }

    private void JumpAfterSwing()
    {
        lineRenderer.enabled = false;
        playerJump.gravity = playerJump.originalGravity;
        player.velocity.y = jumpPowerAfterSwing.y;
        player.velocity.x = jumpPowerAfterSwing.x * swingingDirection.x;
        player.velocity.x = Mathf.Clamp(player.velocity.x, -jumpPowerAfterSwing.x, jumpPowerAfterSwing.x);
        EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", player.velocity.x);
        swingingDirection = Vector2.zero;
    }
}