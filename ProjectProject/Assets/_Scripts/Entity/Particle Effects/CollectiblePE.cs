using UnityEngine;

public class CollectiblePE : MonoBehaviour
{
    private ParticleSystem ps;

    private void Start()
    {
        ps = GetComponent<ParticleSystem>();
        ps.transform.Rotate(90,0,0);
    }

    private void Update()
    {
        if (!ps.isEmitting)
        {
            Destroy(gameObject);
        }
    }
}
