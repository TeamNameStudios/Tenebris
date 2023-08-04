using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ManifestationTrigger : MonoBehaviour
{
    [SerializeField] protected GameObject GO;

    private void Start()
    {
        //Instantiate(GO, transform.position, Quaternion.identity);
        //ManifestationsFactory.Instance.CreateObject(GO.name, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }

    protected virtual void OnDrawGizmos()
    {
        Gizmos.color = GO.GetComponent<SpriteRenderer>().color;
    }
}
