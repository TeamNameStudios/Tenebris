using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerTrigger : ManifestationTrigger
{
    [SerializeField] private float spawnPosX;
    [SerializeField] private Vector2 boxSize;

    private bool playerFound = false;

    protected override void Start()
    {
        
    }

    private void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, boxSize, 0);

        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponent<Player>() && !playerFound)
            {
                playerFound = true;
                Transform player = colliders[i].transform;
                ManifestationsFactory.Instance.CreateObject(GO.name, new Vector3(spawnPosX, player.position.y, 0), Quaternion.identity);
                EventManager<Transform>.Instance.TriggerEvent("onPlayerDetected", player);
                Destroy(gameObject);
            }
        }
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (GO != null)
        {
            Gizmos.DrawWireCube(transform.position, boxSize);
        }   
    }
}
