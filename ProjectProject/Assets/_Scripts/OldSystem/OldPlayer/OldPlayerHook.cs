using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using static UnityEditor.PlayerSettings;
using static UnityEngine.GraphicsBuffer;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class OldPlayerHook : MonoBehaviour
{
    [SerializeField]
    private LineRenderer lineRenderer;
    [SerializeField]
    private HookableObject HookableObject;
    [SerializeField]
    public bool isHooked;
    //[SerializeField]
    //private float minDistance;
    //[SerializeField]
    //private float actualDistance;
    //[SerializeField]
    //private float maxDistance;
    [SerializeField]
    private OldPlayerJump playerJump;
    [SerializeField]
    private OldPlayerMovement playerMovement;
    [SerializeField]
    private OldPlayer player;


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
    [SerializeField]
    LayerMask hookableLayer;

    [SerializeField] private CorruptionSystem corruptionSystem;
    [Tooltip("Value to add only once if the corruptionOverTime box is CHECKED")]
    [SerializeField] private float hookCorruptionOverTime;
    [Tooltip("Value to add only once if the corruptionOverTime box is UNCHECKED")]
    [SerializeField] private float hookCorruptionOnce;
    [Tooltip("Check this box if the corruption should grow as long as the player is holding the grapple button\nUncheck this box if the corruption should grow only once")]
    [SerializeField] private bool corruptionOverTime;
    
    private bool isHooking = false;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        playerJump = GetComponent<OldPlayerJump>();
        playerMovement = GetComponent<OldPlayerMovement>();
        player = GetComponent<OldPlayer>();
        corruptionSystem = GetComponent<CorruptionSystem>();
    }

    private void OnEnable()
    {
        EventManager<bool>.Instance.StartListening("hook", Hook);
    }
    private void OnDisable()
    {
        EventManager<bool>.Instance.StopListening("hook", Hook);
    }

    public HookableObject GetNearestHookable(Collider2D[] hookableHits)
    {
        Vector2 pos = transform.position;
        HookableObject nearestHookableObject = null;
        float minDist = Mathf.Infinity;
        for(int i = 0;i< hookableHits.Length; i++)
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
    private void Update()
    {
        Vector2 pos = transform.position;
        Collider2D[] HookableHits = Physics2D.OverlapCircleAll(pos, 20f);
        HookableObject = GetNearestHookable(HookableHits);

        if (isHooked && !corruptionSystem.corrupted && HookableObject != null)
        {
            if (!corruptionOverTime)
            {
                if (!isHooking)
                {
                    EventManager<float>.Instance.TriggerEvent("Corruption", hookCorruptionOnce);
                    isHooking = true;
                }
            }
            else
            {
                EventManager<float>.Instance.TriggerEvent("Corruption", hookCorruptionOverTime);
            }
            Vector2 hookObjectpos = HookableObject.transform.position;
            lineRenderer.SetPosition(0, hookObjectpos);
            lineRenderer.SetPosition(1, new Vector2(pos.x, pos.y+1));
            player.velocity.x += swingAccellerationSwing * swingingDirection.x * 100f;
            player.velocity.x = Mathf.Clamp(player.velocity.x, -maxSwingPower, maxSwingPower);
            //pos.y = -Mathf.Sqrt(Mathf.Pow(minDistance, 2)- Mathf.Pow(hookObjectpos.x - pos.x, 2)) + hookObjectpos.y;
            pos.y = Mathf.Lerp(pos.y, hookObjectpos.y, Time.fixedDeltaTime * 2f);
            transform.position = Vector2.Lerp(transform.position, pos, Time.fixedDeltaTime * 100f);
            EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", player.velocity.x);
            if ((swingingDirection.x > 0 && hookObjectpos.x < pos.x) || (swingingDirection.x < 0 && hookObjectpos.x > pos.x) || corruptionSystem.corrupted)
            {
                isHooked = false;
                isHooking = false;
                JumpAfterSwing();
            }

        }
    }

    private void Hook(bool _isHooked)
    {
     
        if (_isHooked && !corruptionSystem.corrupted)
        {
            if (HookableObject != null)
                
            {
                Vector2 pos = transform.position;
                Vector2 hookObjectpos = HookableObject.transform.position;
                swingingDirection = playerMovement.isFacingRight ? Vector2.right : Vector2.left;
                if ((swingingDirection.x > 0 && hookObjectpos.x < pos.x) || (swingingDirection.x < 0 && hookObjectpos.x > pos.x))
                {
                    return;
                }
                //float distance = Vector2.Distance(HookableObject.transform.position, transform.position);
                //actualDistance = Mathf.Clamp(distance - 3f, minDistance, maxDistance);
                isHooked = _isHooked;
                //player.isGrounded = false;
                //playerJump.gravity = 0f;
                player.velocity.y = 0f;
                lineRenderer.enabled = true;
            }   
        }
        else
        {
            lineRenderer.enabled = false;
            //playerJump.gravity = playerJump.originalGravity;
        }
        
    }

    private void JumpAfterSwing()
    {
        lineRenderer.enabled = false;
        //playerJump.gravity = playerJump.originalGravity;
        player.velocity.y = jumpPowerAfterSwing.y;
        player.velocity.x = jumpPowerAfterSwing.x * swingingDirection.x;
        player.velocity.x = Mathf.Clamp(player.velocity.x, -jumpPowerAfterSwing.x, jumpPowerAfterSwing.x);
        EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", player.velocity.x);
        swingingDirection = Vector2.zero;
    }
}