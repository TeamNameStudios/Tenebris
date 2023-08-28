using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shadow : MapMover, IEnemy
{
    [SerializeField]
    private float shadowSpeed = 1.5f;    
    [SerializeField]
    private float shadowSpeedIncrement = 1f;
    [SerializeField]
    private Player player;


    [SerializeField] private float minDistance;
    [SerializeField] private float corruptionValue;
    [SerializeField] private float insideShadowCorruptionValue;

    [SerializeField] private float maxDistance = 35;
    [SerializeField] private float changeMaxDistanceTimer;
    private bool canChange = true;


    // Update is called once per frame

    protected override void OnEnable()
    {
        base.OnEnable();
        EventManager<bool>.Instance.StartListening("onLevelUp", LevelUp);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EventManager<bool>.Instance.StopListening("onLevelUp", LevelUp);
    }

    public override void Update()
    {
        if (GameController.Instance.State == GameState.PLAYING)
        {
            base.Update();

            Vector2 pos = transform.position;
            
            if (canChange)
            {
                canChange = false;
                float previousDistance = maxDistance;
                maxDistance = Random.Range(20, 35);
                //pos = Vector2.Lerp(pos, new Vector2(maxDistance, pos.y), 10f * Time.deltaTime);
                StartCoroutine(ChangeMaxDistance());
            }


            pos.x += Vector2.right.x * shadowSpeed * Time.deltaTime;
            if (player.transform.position.x - pos.x > maxDistance)
            {
                pos = Vector2.Lerp(pos, new Vector2(player.transform.position.x - maxDistance, pos.y), 10f * Time.deltaTime);
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
    public void Setup(Player _player)
    {
        player = _player;
    }

    private void LevelUp(bool value)
    {
        shadowSpeed += shadowSpeedIncrement;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(transform.position, new Vector3(transform.position.x + minDistance, transform.position.y, transform.position.z));
    }

    private IEnumerator ChangeMaxDistance()
    {
        yield return new WaitForSeconds(changeMaxDistanceTimer);
        canChange = true;
    }
}
