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

    protected override void OnEnable()
    {
        base.OnEnable();
        // NOT NEEDED FOR NOW
        //StartCoroutine(AutoDestruction());
        //EventManager<float>.Instance.StartListening("onPlayerChangeXVelociy", ChangeVelocity);
        //EventManager<Vector2>.Instance.StartListening("onPlayerChangeDirection", PlayerGoingRight);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        //StopAllCoroutines();
        //EventManager<float>.Instance.StopListening("onPlayerChangeXVelociy", ChangeVelocity);
        //EventManager<Vector2>.Instance.StopListening("onPlayerChangeDirection", PlayerGoingRight);
    }

    private void ChangeVelocity(float value)
    {
        playerVelocity = value;
    }

    private void PlayerGoingRight(Vector2 direction)
    {
        playerDirection = direction;
    }

    protected IEnumerator AutoDestruction()
    {
        yield return new WaitForSeconds(DestroyTimer);
        //Destroy(gameObject);
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
