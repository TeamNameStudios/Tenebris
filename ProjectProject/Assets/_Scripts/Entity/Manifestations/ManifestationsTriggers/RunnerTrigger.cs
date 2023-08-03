using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunnerTrigger : ManifestationTrigger
{
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        if (GO != null)
        {
            Gizmos.DrawCube(transform.position, GO.transform.localScale);
            Gizmos.DrawWireCube(transform.position, new Vector3(GO.GetComponent<Runner>().boxSize.x, GO.GetComponent<Runner>().boxSize.y, 0));
        }   
    }
}
