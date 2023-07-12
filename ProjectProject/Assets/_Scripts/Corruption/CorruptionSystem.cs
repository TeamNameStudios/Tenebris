using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionSystem : MonoBehaviour
{
    public float Corruption;
    
    [SerializeField] private float maxCorruption;

    private Player player;

    public bool corrupted = false;
    [SerializeField] private float corruptionTime;
    [SerializeField] private bool canRecover;
    [SerializeField] private float recoverCorruptionWaitTime;
    [SerializeField] private float recoverCorruptionSpeed;

    private void OnEnable()
    {
        EventManager<float>.Instance.StartListening("Corruption", AddCorruption);
    }

    private void OnDisable()
    {
        EventManager<float>.Instance.StopListening("Corruption", AddCorruption);
    }

    private void Start()
    {
        player = GetComponent<Player>();
    }

    private void Update()
    {
        if (corrupted)
        {
            StartCoroutine(CorruptionCoroutine());
            //  while he is corrupted we can slow him down, make him unable to use his powers and maybe make the shadow faster
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            AddCorruption(5);
        }
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
        StopAllCoroutines();

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
                StartCoroutine(CanRecovery());
            }

            EventManager<float>.Instance.TriggerEvent("UpdateCorruptionBar", Corruption);

            if (Corruption >= maxCorruption)
            {
                StopCoroutine(CanRecovery());
                corrupted = true;
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
        yield return new WaitForSeconds(corruptionTime);
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
