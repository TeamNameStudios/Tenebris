using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class LandableGround : MonoBehaviour
{
    public float groundHeight;
    private BoxCollider2D boxCollider;
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        groundHeight = (transform.position.y + transform.localScale.y / 2);
    }

}
