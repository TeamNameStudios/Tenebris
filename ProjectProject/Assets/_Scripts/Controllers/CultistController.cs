using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CultistController : Singleton<CultistController>
{
    public Cultist cultist;
    public void SpawnCultist(CultistType cultistType) {
        SpawnPlayer(cultistType,Vector2.zero);
    }
    private void SpawnPlayer (CultistType cultistType, Vector2 pos) {
       ScriptableCultist scriptableCultist = ResourceSystem.Instance.GetCultist(cultistType);

       GameObject cultistGO = Instantiate(scriptableCultist.CultistPrefab, pos, Quaternion.identity);
       Cultist _cultist = cultistGO.GetComponent<Cultist>();
       _cultist.Setup(scriptableCultist);
       cultist = _cultist;
    }   

    public void ClearPlayer()
    {
        if(cultist != null)
        {
            Destroy(cultist.gameObject);
        }
    }
}
