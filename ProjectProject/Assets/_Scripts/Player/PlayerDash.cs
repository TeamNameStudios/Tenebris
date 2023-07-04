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
    private Player player;

    private void Awake()
    {
        player = GetComponent<Player>();
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
        if(canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        EventManager<bool>.Instance.TriggerEvent("isDashing", isDashing);
        float originalGravity = player.gravity;
        player.gravity = 0f;
        float velocityX = player.velocity.x * dashingPower;
        player.velocity.x = velocityX;
        player.velocity.y = 0f;
        EventManager<float>.Instance.TriggerEvent("onPlayerChangeXVelociy", velocityX);
        yield return new WaitForSeconds(dashingTime);
        isDashing =false;
        player.gravity = originalGravity;
        EventManager<bool>.Instance.TriggerEvent("isDashing", isDashing);
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}
