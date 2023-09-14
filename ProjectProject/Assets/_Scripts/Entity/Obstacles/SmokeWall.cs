using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeWall : MonoBehaviour
{
    private BoxCollider2D boxCollider2D;

    private void Awake()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
    }
    private void OnEnable()
    {
        EventManager<bool>.Instance.StartListening("isDashing", ManageDash);
        EventManager<bool>.Instance.StartListening("isFinishDashed", ManageStopDash);
    }
    private void OnDisable()
    {
        EventManager<bool>.Instance.StopListening("isDashing", ManageDash);
        EventManager<bool>.Instance.StopListening("isFinishDashed", ManageStopDash);
    }

    private void ManageDash(bool isDashing)
    {
        int LayerDefault = LayerMask.NameToLayer("Default");
        gameObject.layer = LayerDefault;
        boxCollider2D.enabled = false;
    }
    private void ManageStopDash(bool isDashing)
    {
        boxCollider2D.enabled = true;
        int LayerGround = LayerMask.NameToLayer("Ground");
        gameObject.layer = LayerGround;
    }
}
