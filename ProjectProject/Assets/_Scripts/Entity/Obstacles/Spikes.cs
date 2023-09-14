using UnityEngine;

public class Spikes : MonoBehaviour, IEnemy
{
    [SerializeField] private float spikeCorruptionOnStay;
    [SerializeField] private float spikeCorruptionOnHit;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        EventManager<SoundEnum>.Instance.TriggerEvent("onPlayClip", SoundEnum.hitSound);
        EventManager<float>.Instance.TriggerEvent("Corruption", spikeCorruptionOnHit);
        EventManager<bool>.Instance.TriggerEvent("onHit", true);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        EventManager<float>.Instance.TriggerEvent("Corruption", spikeCorruptionOnStay);
    }

}
