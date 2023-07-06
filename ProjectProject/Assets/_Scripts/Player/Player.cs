using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.U2D.Path.GUIFramework;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Player : MonoBehaviour
{

    [SerializeField]
    public Vector2 velocity;
    [SerializeField]
    public Vector2 direction = Vector2.zero;
    [SerializeField]
    public bool isGrounded = false;
    [SerializeField]
    public bool isDashing = false;
    


}
