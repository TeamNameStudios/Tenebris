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
    GameObject DashEffect;

    [SerializeField] private CorruptionSystem corruptionSystem;
    [SerializeField] private float dashCorruption;

    private void Awake()
    {
        playerJump = GetComponent<PlayerJump>();
        player = GetComponent<Player>();
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

    private IEnumerator Dash()
    {
        EventManager<float>.Instance.TriggerEvent("Corruption", dashCorruption);
        canDash = false;
        GameObject dashEffect = Instantiate(DashEffect,transform.position, DashEffect.transform.rotation);


        player.isDashing = true;
        float originalGravity = playerJump.gravity;
        playerJump.gravity = 0f;
        float velocityX = player.velocity.x * dashingPower;
        player.velocity.x = velocityX;
        player.velocity.y = 0f;
        EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", velocityX);
        yield return new WaitForSeconds(dashingTime);
        playerJump.gravity = originalGravity;
        Destroy(dashEffect);
        player.isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
