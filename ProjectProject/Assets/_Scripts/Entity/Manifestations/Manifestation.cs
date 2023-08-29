using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Manifestation : MapMover, IEnemy
{
    [SerializeField] protected float CorruptionValue;
    [SerializeField] protected float DestroyTimer;

    protected float playerVelocity;
    protected Vector2 playerDirection;

    [SerializeField] protected bool canDieToShadow;

    protected Coroutine destructionCO;

    [SerializeField] protected AudioSource manifestationSource;
    [SerializeField] protected bool canPlayClip = true;

    private void Awake()
    {
        manifestationSource = GetComponent<AudioSource>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    protected void PlayClip()
    {
        if (!manifestationSource.isPlaying && canPlayClip)
        {
            manifestationSource.PlayOneShot(manifestationSource.clip);
            canPlayClip = false;
        }
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
            EventManager<bool>.Instance.TriggerEvent("onHit", true);
            Debug.Log("COLLIDED!");
        }
        else if (collision.gameObject.CompareTag("Shadow") && canDieToShadow)
        {
            //Destroy(gameObject);
            ManifestationsFactory.Instance.ReturnObject(gameObject);
        }
    }
}
