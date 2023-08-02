using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(BoxCollider2D))]
public class Block : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    
    private void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.usedByComposite = true;
        gameObject.layer = 3;
    }
}
