using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorruptionSystem : MonoBehaviour
{
    public float Corruption;
    
    [SerializeField] private float maxCorruption;

    private Player player;

    public bool corrupted = false;
    public bool invincibility = false;

    [SerializeField] private float fullyCorruptionTime;
    [SerializeField] private bool canRecover;
    [SerializeField] private float recoverCorruptionWaitTime;
    [SerializeField] private float recoverCorruptionSpeed;
    [SerializeField] private float invincibilitySeconds;
    
    [Tooltip("Used to add a value to the recovered corruption, to not make it start from 0,\nKeep the value REALLY small, example: 0.02f")]
    [SerializeField] private float recoveryStartingSmoothness;
    
    public float elapsedTime = 0;
    public bool isRecovering = false;

    private Coroutine recoveryCO;
    private Coroutine corruptionCO;

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

    private void Update()
    {
        if (!corrupted && canRecover && TempGameController.Instance.State == GameState.PLAYING)
        {
            if (Corruption > 0)
            {
                isRecovering = true;
                float normalizedTime = (elapsedTime / maxCorruption) + recoveryStartingSmoothness;
                float easeTime = EaseInCubic(normalizedTime);
                DecreaseCorruption(recoverCorruptionSpeed * easeTime);
                elapsedTime += Time.deltaTime;
                Debug.Log("Recovering " + normalizedTime * recoverCorruptionSpeed + " corruption");
            }
        }

        //temp method to test corruption sys
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            AddCorruption(maxCorruption);
        }
    }

    private void AddCorruption(float value)
    {
        if (recoveryCO != null)
        {
            StopCoroutine(recoveryCO);
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
                isRecovering = false;
                
                recoveryCO = StartCoroutine(CanRecovery());
            }

            EventManager<float>.Instance.TriggerEvent("UpdateCorruptionBar", Corruption);

            if (Corruption >= maxCorruption)
            {
                
                if (recoveryCO != null)
                {
                    StopCoroutine(recoveryCO);
                }

                isRecovering = false;
                invincibility = true;
                corrupted = true;
                corruptionCO = StartCoroutine(CorruptionCoroutine());
            }
        }
    }

    private void DecreaseCorruption(float value)
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

        //stop corruption coroutine and reset corrupted state
        if (recoveryCO != null)
        {
            StopCoroutine(recoveryCO);
        }
        
        if (corruptionCO != null)
        {
            StopCoroutine(corruptionCO);
            corrupted = false;
            invincibility = false;
            recoveryCO = StartCoroutine(CanRecovery());
        }
        
        EventManager<float>.Instance.TriggerEvent("UpdateCorruptionBar", Corruption);
    }

    private IEnumerator CorruptionCoroutine()
    {
        yield return new WaitForSeconds(invincibilitySeconds);
        invincibility = false;
        yield return new WaitForSeconds(fullyCorruptionTime);
        corrupted = false;
        //Corruption = 0;

        yield return new WaitForSeconds(.2f);
        canRecover = true;
        isRecovering = true;
        elapsedTime = 0;

        EventManager<float>.Instance.TriggerEvent("UpdateCorruptionBar", Corruption);
    }

    private IEnumerator CanRecovery()
    {
        yield return new WaitForSeconds(recoverCorruptionWaitTime);
        canRecover = true;
        elapsedTime = 0;
    }

    private float EaseInCubic(float t)
    {
        return t * t * t;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Collectible>())
        {
            if (!isRecovering)
            {
                canRecover = false;
            }
        }
    }
}
