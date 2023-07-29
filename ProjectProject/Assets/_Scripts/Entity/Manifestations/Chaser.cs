using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : Manifestation
{
    private bool canStart = false;
    private bool canPursue = false;
    private Transform player;
    private CapsuleCollider2D capsuleCollider;

    [SerializeField] private float distance;
    [SerializeField] private float delay;
    [SerializeField] private float velocity;
    private List<float> playerPos = new List<float>();

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        if (player == null)
        {
            FindPlayer();
        }
        
        if (player != null && Mathf.Abs(transform.position.x - player.position.x) >= distance)
        {
            // here the chaser should "jump" to the ground
            
            canStart = true;
        }
        
        if (canStart)
        {
            // here the chaser should chase the player and replicate his exact movements
        }


    }

    private void FindPlayer()
    {
        RaycastHit2D[] hits = Physics2D.CapsuleCastAll(transform.position, capsuleCollider.size, capsuleCollider.direction, 0, Vector2.down);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.GetComponent<Player>())
            {
                Debug.Log("PLAYER DETECTED");
                player = hits[i].transform;
            }
        }
    }

    private float EaseInQuad(float value)
    {
        return value * value;
    }
}
