using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [SerializeField]
    private bool canDash = true;
    [SerializeField]
    private bool isDashing;
    [SerializeField]
    private float dashingPower = 24f;
    [SerializeField]
    private float dashingTime = 0.2f;
    [SerializeField]
    private float dashingCooldown = 1f;
    [SerializeField]
    private PlayerJump playerJump;
    [SerializeField]
    private Player player;
    [SerializeField]
    private PlayerMovement playerMovement;  
    [SerializeField]
    GameObject DashEffect;

    [SerializeField] private CorruptionSystem corruptionSystem;
    [SerializeField] private float dashCorruption; // value added only when the dash starts

    private void Awake()
    {
        playerJump = GetComponent<PlayerJump>();
        player = GetComponent<Player>();
        playerMovement = GetComponent<PlayerMovement>();
        corruptionSystem = GetComponent<CorruptionSystem>();
    }
    private void OnEnable()
    {
        EventManager<bool>.Instance.StartListening("dash", GoDash);
    }
    private void OnDisable()
    {
        EventManager<bool>.Instance.StopListening("dash", GoDash);
    }

    private void GoDash(bool dash)
    {
        if(canDash && !corruptionSystem.corrupted)
        {
            StartCoroutine(Dash());
        }
    }

    private void Update()
    {
        if (player.isDashing && player._colRight && player._colLeft)
        {
            player.isDashing = false;
            player.velocity.x = 0;
            EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", player.velocity.x);
        }
    }
    private IEnumerator Dash()
    {
        EventManager<float>.Instance.TriggerEvent("Corruption", dashCorruption);
        canDash = false;
        GameObject dashEffect = Instantiate(DashEffect,transform.position + Vector3.up, DashEffect.transform.rotation);
        player.isDashing = true;
        float dashDirection = playerMovement.isFacingRight ? Vector2.right.x : Vector2.left.x;
        Vector3 localScale = dashEffect.transform.localScale;
        localScale.z = dashDirection;
        dashEffect.transform.localScale = localScale;
        //playerJump.gravity = 0f;
        float velocityX = dashDirection * dashingPower;
        player.velocity.x = velocityX;
        player.velocity.y = 0f;
        EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", velocityX);
        yield return new WaitForSeconds(dashingTime);
        player.velocity.x = 0;
        EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", player.velocity.x);
        //playerJump.gravity = playerJump.originalGravity;
        Destroy(dashEffect);
        player.isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);

        canDash = true;
    }
}
