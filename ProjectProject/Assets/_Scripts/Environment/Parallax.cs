using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
    [SerializeField]
    float depth = 1;
    Player player;
    public void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
    }

    private void FixedUpdate()
    {
        float realVelocity = player.velocity.x * depth;
        Vector2 pos = transform.position;
        pos.x -= realVelocity * Time.fixedDeltaTime;
        if (pos.x <= -56)
        {
            pos.x = 56;
        }

        transform.position = pos;
    }
}
