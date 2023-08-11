using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour, IEnemy
{
    [SerializeField] private float spikeCorruption;

    private void OnTriggerStay2D(Collider2D collision)
    {
        EventManager<float>.Instance.TriggerEvent("Corruption", spikeCorruption);
    }

}
