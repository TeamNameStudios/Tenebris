using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : Manifestation
{
    public enum ChaserState { IDLE, DESCENDING, CHASING, ATTACKING}
    public ChaserState state = ChaserState.IDLE;
    
    private bool canPursue = false;
    private bool canAttack = false;

    private Transform player;
    private CapsuleCollider2D capsuleCollider;

    [SerializeField] private float minDistance;
    [SerializeField] private float chaseVelocity;
    [SerializeField] private float attackVelocity;
    [SerializeField] private float maxVelocity;
    [SerializeField] private float pursueTimer;

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
                
                break;

            case ChaserState.DESCENDING:
                
                Vector2 pos = transform.position;
                pos.y = groundHeight + capsuleCollider.size.y / 2;
                transform.position = pos;

                if(transform.position.y - groundHeight <= 1.2f && Mathf.Abs(transform.position.x - player.position.x) > minDistance)
                {
                    state = ChaserState.CHASING;
                }

                break;

            case ChaserState.CHASING:

                //StartCoroutine(PursueCO());
                Vector2 position = transform.position;
                position.x += Vector2.right.x * chaseVelocity * Time.deltaTime;
                transform.position = position;
                chaseVelocity += .02f;
                attackDirection = (player.position - transform.position).normalized;

                if (chaseVelocity >= maxVelocity)
                {
                    state = ChaserState.ATTACKING;
                    attackVelocity = chaseVelocity;
                }

                break;
            
            case ChaserState.ATTACKING:

                transform.position += attackDirection * attackVelocity * Time.deltaTime;

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
                Debug.Log("PLAYER DETECTED");
                player = hits[i].transform;
                state = ChaserState.DESCENDING;
            }
            else
            {
                groundHeight = hits[i].point.y;
            }
        }
    }

    private IEnumerator PursueCO()
    {
        Vector2 pos = transform.position;
        float distance = Vector2.Distance(pos, player.position);
        pos.x += Vector2.right.x * chaseVelocity * Time.deltaTime;
        transform.position = pos;
        chaseVelocity += .02f;
        attackDirection = (player.position - transform.position).normalized;
        yield return new WaitForSeconds(pursueTimer);
        state = ChaserState.ATTACKING;
        attackVelocity = chaseVelocity;
    }
}
