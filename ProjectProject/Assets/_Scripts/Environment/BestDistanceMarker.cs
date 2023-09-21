using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BestDistanceMarker : MapMover
{
    [SerializeField]
    private LayerMask _groundMask;
    public void Start()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 30f, _groundMask);
        if (hit.collider != null)
        {
            transform.position = new Vector2(transform.position.x, hit.collider.transform.position.y +2);
        }
        else
        {
            RaycastHit2D hitUp = Physics2D.Raycast(transform.position, Vector2.up, 30f, _groundMask);
            if (hitUp.collider != null)
            {
                transform.position = new Vector2(transform.position.x, hitUp.collider.transform.position.y + 2);
            }
            else
            {
                transform.position = new Vector2(transform.position.x,-17);
            }

        }
    }
}
