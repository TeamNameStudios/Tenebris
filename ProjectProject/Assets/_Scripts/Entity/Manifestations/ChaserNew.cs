using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserNew : Manifestation
{
    public enum ChaserState { IDLE, CHASING, ATTACKING }
    public ChaserState state = ChaserState.IDLE;

    private Transform player;
    private CapsuleCollider2D capsuleCollider;

    private Vector3 attackDirection;

    [Tooltip("This number determines how much the player must distance himself from the chaser before it starts chasing the player")]
    [SerializeField] private float startDistance;
    [Tooltip("This number determines how close the chaser must be to the player before it locks in a direction")]
    [SerializeField] private float attackDistance;
    private float chaseVelocity;
    [SerializeField] private float startVelocity;
    [Tooltip("Keep this value small, ex. 0.2")]
    [SerializeField] private float velocityAcceleration;

    protected override void OnEnable()
    {
        base.OnEnable();

        player = null;
        state = ChaserState.IDLE;
        chaseVelocity = startVelocity;
    }

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();

    }

    public override void Update()
    {
        base.Update();

        switch (state)
        {
            case ChaserState.IDLE:
                FindPlayer();
                if (player != null && Mathf.Abs(transform.position.x - player.position.x) > startDistance)
                {
                    state = ChaserState.CHASING;
                }
                break;

            case ChaserState.CHASING:
                attackDirection = (player.position - transform.position).normalized;
                transform.position += attackDirection * chaseVelocity * Time.deltaTime;
                chaseVelocity += velocityAcceleration;

                if (Vector2.Distance(transform.position, player.position) <= attackDistance)
                {
                    state = ChaserState.ATTACKING;
                }
                break;

            case ChaserState.ATTACKING:
                transform.position += attackDirection * chaseVelocity * Time.deltaTime;
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
        }
    }
}
