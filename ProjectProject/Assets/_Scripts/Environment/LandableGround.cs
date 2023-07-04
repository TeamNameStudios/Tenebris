using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LandableGround : MonoBehaviour
{
    public float groundHeight;
    private BoxCollider2D collider;
    void Start()
    {
        collider = GetComponent<BoxCollider2D>();
        groundHeight = (transform.position.y + transform.localScale.y / 2);
    }

}
