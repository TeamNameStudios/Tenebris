using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] float corruptionReductionValue;
    [SerializeField] int pageCount;
    [SerializeField] SoundEnum sound;
    [SerializeField] ParticleSystem pickup;

    public float CorruptionReductionValue { get => corruptionReductionValue; private set => corruptionReductionValue = value; }
    public int PageCount { get => pageCount; private set => pageCount = value; }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            EventManager<Collectible>.Instance.TriggerEvent("onCollectiblePickup", this);
            EventManager<SoundEnum>.Instance.TriggerEvent("onPlayClip", sound);

            Instantiate(pickup, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
