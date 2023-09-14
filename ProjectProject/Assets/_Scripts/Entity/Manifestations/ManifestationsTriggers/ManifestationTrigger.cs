using UnityEngine;

public abstract class ManifestationTrigger : MonoBehaviour
{
    [SerializeField] protected GameObject GO;

    protected virtual void Start()
    {
        ManifestationsFactory.Instance.CreateObject(GO.name, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = GO.GetComponent<SpriteRenderer>().color;
    }
}
