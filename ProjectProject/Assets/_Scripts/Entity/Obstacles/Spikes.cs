using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour, IEnemy
{
    [SerializeField] private float spikeCorruption;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EventManager<bool>.Instance.TriggerEvent("onHit", true);
        EventManager<float>.Instance.TriggerEvent("Corruption", spikeCorruption);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        EventManager<float>.Instance.TriggerEvent("Corruption", spikeCorruption);
    }

}
