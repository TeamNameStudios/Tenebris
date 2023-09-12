using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Runner : Manifestation
{
    [SerializeField] private float runnerVelocity;
    [SerializeField] private float runnerVelocityIncrement;

    [SerializeField] private float pursueTime;
    private bool canPursue = true;

    private Transform player;

    [SerializeField] private float delay;
    private List<float> playerPos = new List<float>();
    private bool canGo = false;



    protected override void OnEnable()
    {
        base.OnEnable();
        player = null;

        canGo = false;
        canPursue = true;
        if (playerPos.Count > 0)
        {
            playerPos.Clear();
        }

        EventManager<Transform>.Instance.StartListening("onPlayerDetected", SetPlayer);
        EventManager<bool>.Instance.StartListening("onLevelUp", LevelUp);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        EventManager<Transform>.Instance.StopListening("onPlayerDetected", SetPlayer);
    }

    public override void Update()
    {
        base.Update();


        if (player != null)
        {
            StartCoroutine(PursueCO());
            Vector3 pos = transform.position;
            

            if (canPursue)
            {
                pos.x -= Vector2.right.x * runnerVelocity * Time.deltaTime;
                
                if (canGo)
                {
                    pos.y = playerPos[0];
                    playerPos.RemoveAt(0);
                }
            }
            else if (!canPursue)
            {
                pos.x -= Vector2.right.x * runnerVelocity * Time.deltaTime;
            }

            transform.position = pos;
        }
    }

    private IEnumerator PursueCO()
    {
        playerPos.Add(player.transform.position.y);
        yield return new WaitForSeconds(delay);
        canGo = true;
        yield return new WaitForSeconds(pursueTime + delay);
        canPursue = false;
    }

    private void SetPlayer(Transform _player)
    {
        player = _player;
        PlayClip();
    }

    private void LevelUp(bool value)
    {
        runnerVelocity += runnerVelocityIncrement;
    }


}
