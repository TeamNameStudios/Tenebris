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
    private PlayerJump playerJump;
    [SerializeField]
    private Player player;
    [SerializeField]
    float swingPower;
    [SerializeField]
    float distanceToCheck;
    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        playerJump = GetComponent<PlayerJump>();
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

        //if (HookableObject != null)
        //{
            //if (Vector3.Distance(HookableObject.transform.position, pos) > 10f)
            //{
            //    lineRenderer.enabled = false;
            //    HookableObject = null;

            //}
        //}
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
            playerJump.gravity = 0f;
            float velocityX = player.direction.x * swingPower;
            player.velocity.x = velocityX;
            pos.y = -Mathf.Sqrt(Mathf.Pow(minDistance, 2)- Mathf.Pow(hookObjectpos.x - pos.x, 2)) + hookObjectpos.y;
            transform.position = Vector2.Lerp(transform.position, pos, Time.fixedDeltaTime * 100f);
            EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", velocityX);
        }
    }

    private void Hook(bool _isHooked)
    {

        this.isHooked = _isHooked;
        if(isHooked)
        {
            lineRenderer.enabled = true;
        }
        else
        {
            lineRenderer.enabled = false;
            playerJump.gravity = playerJump.originalGravity;
        }


    }
}