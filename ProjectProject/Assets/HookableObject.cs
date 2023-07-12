using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HookableObject : MonoBehaviour
{
    public Collider2D collider;
    void Awake()
    {
        collider = GetComponent<Collider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
