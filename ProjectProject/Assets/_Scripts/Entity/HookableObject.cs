using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookableObject : MonoBehaviour
{
    public Collider2D hookCollider;
    public GameObject grappleMarker;
    void Awake()
    {
        hookCollider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnableGrapplable() {
        grappleMarker.SetActive(true);
    }

    public void DisableGrapplable()
    {
        grappleMarker.SetActive(false);
    }
}
