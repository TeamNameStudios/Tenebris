using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : Manifestation
{
    [SerializeField] private float runnerVelocity;
    [SerializeField] public Vector2 boxSize;

    [SerializeField] private bool canStart = false;
    [SerializeField] private float pursueTime;
    private bool canPursue = true;

    private Player player;

    [SerializeField] private float delay;
    private List<float> playerPos = new List<float>();
    private bool canGo = false;

    protected override void OnEnable()
    {
        base.OnEnable();

        canGo = false;
        player = null;
        canStart = false;
        canPursue = true;
        if (playerPos.Count > 0)
        {
            playerPos.Clear();
        }
    }


    public override void Update()
    {
        base.Update();

        if (!canStart)
        {
            CanStart();
        }
        
        if (canStart)
        {
            StartCoroutine(PursueCO());
            Vector3 pos = transform.position;

            if (canPursue)
            {
                pos.x -= Vector2.right.x * runnerVelocity * Time.deltaTime;
                //StartCoroutine(CO());
                
                if (canGo)
                {
                    pos.y = playerPos[0];
                    playerPos.RemoveAt(0);
                }
                //pos.y = player.transform.position.y;
            }
            else if (!canPursue)
            {
                pos.x -= Vector2.right.x * runnerVelocity * Time.deltaTime;
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
        playerPos.Add(player.transform.position.y);
        yield return new WaitForSeconds(delay);
        canGo = true;
        yield return new WaitForSeconds(pursueTime + delay);
        canPursue = false;
    }

    //private IEnumerator CO()
    //{
    //    yield return new WaitForSeconds(delay);
    //    canGo = true;
    //}
}
