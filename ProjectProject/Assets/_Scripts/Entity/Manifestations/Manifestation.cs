using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Manifestation : MapMover, IEnemy
{
    [SerializeField] protected float CorruptionValue;
    [SerializeField] protected float DestroyTimer;
    [SerializeField] protected bool canDieToShadow;
    protected Coroutine destructionCO;
    [SerializeField] protected SoundEnum manifestationSound;

    protected void PlayClip()
    {
        EventManager<SoundEnum>.Instance.TriggerEvent("onPlayClip", manifestationSound);
    }

    protected IEnumerator AutoDestruction()
    {
        yield return new WaitForSeconds(DestroyTimer);
        ManifestationsFactory.Instance.ReturnObject(gameObject);
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            EventManager<float>.Instance.TriggerEvent("Corruption", CorruptionValue);
            EventManager<SoundEnum>.Instance.TriggerEvent("onPlayClip", SoundEnum.hitSound);
            EventManager<bool>.Instance.TriggerEvent("onHit", true);
        }
        else if (collision.gameObject.CompareTag("Shadow") && canDieToShadow)
        {
            ManifestationsFactory.Instance.ReturnObject(gameObject);
        }
    }
}
