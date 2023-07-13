using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MapMover
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

    private void FixedUpdate()
    {
        if (GameController.Instance.State == GameState.PLAYING)
        {
            base.FixedUpdate();

            Vector2 pos = transform.position;

            pos.x += Vector2.right.x * shadowSpeed * Time.fixedDeltaTime;
            if (player.transform.position.x - pos.x > 35)
            {
                pos = Vector2.Lerp(pos, new Vector2(player.transform.position.x - 35f, pos.y), 10f * Time.fixedDeltaTime);
            }

            transform.position = pos;

            float distance = Vector2.Distance(pos, player.transform.position);

            if (distance <= minDistance)
            {
                EventManager<float>.Instance.TriggerEvent("Corruption", Mathf.Pow((corruptionValue / distance), 2));
            }
            else if (transform.position.x > player.transform.position.x)
            {
                EventManager<float>.Instance.TriggerEvent("Corruption", insideShadowCorruptionValue);
            }
        }

    }




}