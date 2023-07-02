using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MapMover
{
    float shadowSpeed = 1.5f;
    [SerializeField]
    private Player player;

    // Update is called once per frame
    void FixedUpdate()
    {
        base.FixedUpdate();
        Vector2 pos = transform.position;

        pos.x += Vector2.right.x * shadowSpeed * Time.fixedDeltaTime;
        if (player.transform.position.x - pos.x > 35)
        {
            pos = Vector2.Lerp(pos, new Vector2( player.transform.position.x - 35f, pos.y), 10f * Time.fixedDeltaTime);
        }
        
        transform.position = pos;
    }
}
