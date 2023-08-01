using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : Manifestation
{
    public enum ChaserState { IDLE, DESCENDING, CHASING, ATTACKING}
    public ChaserState state = ChaserState.IDLE;
    

    private Transform player;
    private CapsuleCollider2D capsuleCollider;

    [SerializeField] private float minDistance;
    [SerializeField] private float chaseVelocity;
    [SerializeField] private float attackVelocity;
    [SerializeField] private float maxVelocity;

    private float groundHeight;
    private Vector3 attackDirection;

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();

    }

    private void Update()
    {
        switch (state)
        {
            case ChaserState.IDLE:
                
                FindPlayer();
                if (player != null && Mathf.Abs(transform.position.x - player.position.x) > minDistance)
                {
                    Vector2 pos = transform.position;
                    pos.y = groundHeight + capsuleCollider.size.y / 2;
                    transform.position = pos;
                    state = ChaserState.DESCENDING;
                }
                
                break;

            case ChaserState.DESCENDING:
                


                if (transform.position.y - groundHeight <= 1.2f )
                {
                    state = ChaserState.CHASING;
                }

                break;

            case ChaserState.CHASING:

                Vector2 position = transform.position;
                position.x += Vector2.right.x * chaseVelocity * Time.deltaTime;
                transform.position = position;
                chaseVelocity += .02f;

                if (chaseVelocity >= maxVelocity)
                {
                    attackDirection = (player.position - transform.position).normalized;
                    attackVelocity = chaseVelocity + 10;
                    state = ChaserState.ATTACKING;
                }

                break;
            
            case ChaserState.ATTACKING:

                transform.position += attackDirection * attackVelocity * Time.deltaTime;
                StartCoroutine(AutoDestruction());

                break;
        }
    }

    private void FindPlayer()
    {
        RaycastHit2D[] hits = Physics2D.CapsuleCastAll(transform.position, capsuleCollider.size, capsuleCollider.direction, 0, Vector2.down);

        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].transform.GetComponent<Player>())
            {
                player = hits[i].transform;
            }
            else
            {
                groundHeight = hits[i].point.y;
            }
        }
    }
}
