using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Manifestation : MonoBehaviour
{
    [SerializeField] protected float CorruptionValue;
    [SerializeField] protected float DestroyTimer;

    protected float playerVelocity;
    protected Vector2 playerDirection;

    private void OnEnable()
    {
        //StartCoroutine(AutoDestruction());
        EventManager<float>.Instance.StartListening("onPlayerChangeXVelociy", ChangeVelocity);
        EventManager<Vector2>.Instance.StartListening("onPlayerChangeDirection", PlayerGoingRight);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private void Update()
    {
        
    }

    private void ChangeVelocity(float value)
    {
        playerVelocity = value;
    }

    private void PlayerGoingRight(Vector2 direction)
    {
        playerDirection = direction;
    }

    private IEnumerator AutoDestruction()
    {
        yield return new WaitForSeconds(DestroyTimer);
        ManifestationsFactory.Instance.ReturnObject(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 7)
        {
            EventManager<float>.Instance.TriggerEvent("Corruption", CorruptionValue);
            Debug.Log("COLLIDED!");
        }
    }
}
