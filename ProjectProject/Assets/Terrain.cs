using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;

public class Terrain : MonoBehaviour
{
    public float terrainHeight;
    BoxCollider2D collider;
    void Awake()
    {
        collider = GetComponent<BoxCollider2D>();
        terrainHeight = transform.position.y + transform.localScale.y / 2;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
