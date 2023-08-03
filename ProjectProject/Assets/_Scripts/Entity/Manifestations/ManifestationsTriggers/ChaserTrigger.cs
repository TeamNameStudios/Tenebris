using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaserTrigger : ManifestationTrigger
{
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (GO != null)
        {
            Gizmos.DrawSphere(transform.position, GO.GetComponent<CapsuleCollider2D>().size.x);
        }
    }
}
