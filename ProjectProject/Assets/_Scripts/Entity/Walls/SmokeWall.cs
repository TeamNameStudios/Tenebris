using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeWall : MonoBehaviour
{
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
    }
    private void ManageStopDash(bool isDashing)
    {
        int LayerGround = LayerMask.NameToLayer("Ground");
        gameObject.layer = LayerGround;
    }
}
