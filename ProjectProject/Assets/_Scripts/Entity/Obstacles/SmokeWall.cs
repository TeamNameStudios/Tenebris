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
        EventManager<bool>.Instance.StartListening("onDash", ManageDash);
    }
    private void OnDisable()
    {
        EventManager<bool>.Instance.StopListening("onDash", ManageDash);
    }

    private void ManageDash(bool isDashing)
    {
        if (isDashing)
        {
            int LayerDefault = LayerMask.NameToLayer("Default");
            gameObject.layer = LayerDefault;
            boxCollider2D.enabled = false;
        }
        else {
            boxCollider2D.enabled = true;
            int LayerGround = LayerMask.NameToLayer("Ground");
            gameObject.layer = LayerGround;
        }
    
    }
}
