using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : Manifestation
{
    [SerializeField] private float runnerVelocity;
    [SerializeField] private Vector2 boxSize;

    [SerializeField] private bool canStart = false;
    [SerializeField] private float pursueTime;
    private bool canPursue = true;

    private Player player;
    private Vector2 direction;

    private void Update()
    {
        if (!canStart)
        {
            CanStart();
        }
    }

    private void LateUpdate()
    {
        if (canStart)
        {
            StartCoroutine(PursueCO());
            Vector3 pos = transform.position;

            if (canPursue)
            {
                //float rotAngle = Mathf.Asin((pos.y - player.transform.position.y) / (pos.x - player.transform.position.x));
                //transform.Rotate(new Vector3(0, 0, rotAngle * Mathf.Rad2Deg));

                pos.x -= Vector2.right.x * runnerVelocity * Time.deltaTime;

                //direction = new Vector2((Mathf.Sqrt((pos.x - player.transform.position.x) + (pos.y - player.transform.position.y))), 0);
                //pos.x -= direction.magnitude * runnerVelocity * Time.fixedDeltaTime;
                pos.y = player.transform.position.y;
            }
            else if (!canPursue)
            {
                pos.x -= Vector2.right.x * runnerVelocity * Time.deltaTime;
                
                //pos.x -= direction.magnitude * runnerVelocity * Time.fixedDeltaTime;
            }

            transform.position = pos;
        }
    }

    private void CanStart()
    {
        if (!canStart)
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, boxSize, 0);
            
            for (int i = 0; i < colliders.Length; i++)
            {
                if (colliders[i].GetComponent<Player>())
                {
                    player = colliders[i].GetComponent<Player>();
                    Vector2 pos = transform.position;
                    pos.y = colliders[i].transform.position.y;
                    transform.position = pos;
                    canStart = true;
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, new Vector3(boxSize.x, boxSize.y, 0));
    }

    private IEnumerator PursueCO()
    {
        yield return new WaitForSeconds(pursueTime);
        canPursue = false;
    }
}
