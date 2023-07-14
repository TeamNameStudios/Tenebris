using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionSystem : MonoBehaviour
{
    public float Corruption;
    
    [SerializeField] private float maxCorruption;

    private Player player;

    public bool corrupted = false;
    [SerializeField] private float fullyCorruptionTime;
    [SerializeField] private bool canRecover;
    [SerializeField] private float recoverCorruptionWaitTime;
    [SerializeField] private float recoverCorruptionSpeed;

    //private Queue<Coroutine> coroutines = new Queue<Coroutine>();
    private Coroutine thisCO;

    private void OnEnable()
    {
        EventManager<float>.Instance.StartListening("Corruption", AddCorruption);
        EventManager<float>.Instance.StartListening("onCollectiblePickup", DecreaseCorruption);
    }

    private void OnDisable()
    {
        EventManager<float>.Instance.StopListening("Corruption", AddCorruption);
        EventManager<float>.Instance.StopListening("onCollectiblePickup", DecreaseCorruption);
    }

    private void Start()
    {
        //player = GetComponent<Player>();
    }

    private void FixedUpdate()
    {
        if (!corrupted && canRecover)
        {
            DecreaseCorruption(Mathf.Pow(recoverCorruptionSpeed / Corruption, 2));
        }

        //if (Corruption == 0)
        //{
        //    EventManager<bool>.Instance.TriggerEvent("PlayerCorrupted", false);
        //}
        //else
        //{
        //    EventManager<bool>.Instance.TriggerEvent("PlayerCorrupted", true);
        //
        //}
    }

    private void AddCorruption(float value)
    {
        //StopAllCoroutines();
        //if (coroutines.Count != 0)
        //{
        //    StopCoroutine(coroutines.Peek());
        //    coroutines.Dequeue();
        //}
        if (thisCO != null)
        {
            StopCoroutine(thisCO);
        }

        EventManager<float>.Instance.TriggerEvent("InitCorruptionBar", maxCorruption);

        if (!corrupted)
        {
            if (value + Corruption >= maxCorruption)
            {
                Corruption = maxCorruption;
            }
            else
            {
                Corruption += value;
                canRecover = false;
                //coroutines.Enqueue(StartCoroutine(CanRecovery()));
                
                thisCO = StartCoroutine(CanRecovery());
            }

            EventManager<float>.Instance.TriggerEvent("UpdateCorruptionBar", Corruption);

            if (Corruption >= maxCorruption)
            {
                //StopCoroutine(CanRecovery());
                
                //if (coroutines.Count != 0)
                //{
                //    StopCoroutine(coroutines.Peek());
                //    coroutines.Dequeue();
                //}
                
                if (thisCO != null)
                {
                    StopCoroutine(thisCO);
                }
                
                corrupted = true;
                StartCoroutine(CorruptionCoroutine());
            }
        }
    }

    private void DecreaseCorruption(float value)
    {
        if (!corrupted)
        {
            if (Corruption - value <= 0)
            {
                Corruption = 0;
                // maybe spawn a particle effect ??
            }
            else
            {
                Corruption -= value;
            }

            EventManager<float>.Instance.TriggerEvent("UpdateCorruptionBar", Corruption);
        }
    }

    private IEnumerator CorruptionCoroutine()
    {
        yield return new WaitForSeconds(fullyCorruptionTime);
        corrupted = false;
        Corruption = 0;
        EventManager<float>.Instance.TriggerEvent("UpdateCorruptionBar", Corruption);
    }

    private IEnumerator CanRecovery()
    {
        yield return new WaitForSeconds(recoverCorruptionWaitTime);
        canRecover = true;
    }
}
