using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MapMover, IEnemy
{
    [SerializeField]
    float shadowSpeed = 1.5f;
    [SerializeField]
    private Player player;


    [SerializeField] private float minDistance;
    [SerializeField] private float corruptionValue;
    [SerializeField] private float insideShadowCorruptionValue;

    // Update is called once per frame

    public void Setup(Player _player)
    {
        player = _player;
    }

    public override void Update()
    {
        if (TempGameController.Instance.State == GameState.PLAYING)
        {
            base.Update();

            Vector2 pos = transform.position;

            pos.x += Vector2.right.x * shadowSpeed * Time.deltaTime;
            if (player.transform.position.x - pos.x > 35)
            {
                pos = Vector2.Lerp(pos, new Vector2(player.transform.position.x - 35f, pos.y), 10f * Time.deltaTime);
            }

            transform.position = pos;

            float distance = Vector2.Distance(pos, player.transform.position);

            if (distance <= minDistance)
            {
                float corruption = Mathf.Pow((corruptionValue / distance), 2);
                EventManager<float>.Instance.TriggerEvent("Corruption", corruption);
            }
            else if (transform.position.x > player.transform.position.x)
            {
                EventManager<float>.Instance.TriggerEvent("Corruption", insideShadowCorruptionValue);
            }
        }

    }




}
