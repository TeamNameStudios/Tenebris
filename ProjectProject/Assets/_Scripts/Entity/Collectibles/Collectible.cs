using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] float corruptionReductionValue;
    [SerializeField] int pageCount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            EventManager<float>.Instance.TriggerEvent("onCollectiblePickup", corruptionReductionValue);
            EventManager<int>.Instance.TriggerEvent("onCollectiblePickup", pageCount);

            Destroy(gameObject);
        }
    }
}
