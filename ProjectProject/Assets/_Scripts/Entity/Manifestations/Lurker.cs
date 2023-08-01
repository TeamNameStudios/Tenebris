using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lurker : Manifestation
{
    private enum LurkerState { IDLE, CHASING, FALLING}
    private LurkerState state = LurkerState.IDLE;
    
    private Transform player;
    private CapsuleCollider2D capsuleCollider;

    [SerializeField] private float fallTime;
    [SerializeField] private float fallVelocity;

    private float elapsedTime = 0;
    private float landingPoint;

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider2D>();
    }

    private void Update()
    {
        switch (state)
        {
            case LurkerState.IDLE:
                FindPlayer();
                break;

            case LurkerState.CHASING:
                Vector2 pos = transform.position;
                pos.x = player.position.x + 1.5f;
                transform.position = pos;
                elapsedTime += Time.deltaTime;

                if (elapsedTime >= fallTime)
                {
                    landingPoint = FindLandingPoint();
                    state = LurkerState.FALLING;
                }
                break;

            case LurkerState.FALLING:
                Vector2 position = transform.position;
                position.y += Vector2.down.y * fallVelocity * Time.deltaTime;
                if (transform.position.y <= landingPoint)
                {
                    fallVelocity = 0;
                    position.y = landingPoint + capsuleCollider.size.y / 2;
                }

                transform.position = position;
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
                state = LurkerState.CHASING;
            }
        }
    }

    private float FindLandingPoint()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down);
        for (int i = 0; i < hits.Length; i++)
        {
            if (!hits[i].transform.GetComponent<Player>() && !hits[i].transform.GetComponent<Manifestation>())
            {
                return hits[i].point.y;
            }
        }

        return 0;
    }
}
