using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] float corruptionReductionValue;
    [SerializeField] int pageCount;

    [SerializeField] AudioClip collectibleClip;
    [SerializeField] ParticleSystem pickup;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            EventManager<float>.Instance.TriggerEvent("onCollectiblePickup", corruptionReductionValue);
            EventManager<int>.Instance.TriggerEvent("onCollectiblePickup", pageCount);
            EventManager<AudioClip>.Instance.TriggerEvent("onPlayClip", collectibleClip);

            Instantiate(pickup, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
