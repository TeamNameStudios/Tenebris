using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TerrainGenerator : Singleton<TerrainGenerator>
{
    [SerializeField]
    private GameObject TerrainPrefab;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public List<Terrain> GenerateTerrains(Transform parentTransform)
    {
        
        GameObject terrainContainer = new GameObject("Terrains");
        terrainContainer.transform.position = parentTransform.position;
        terrainContainer.transform.SetParent(parentTransform);
        List <Terrain> terrains = new List <Terrain>();
        for (int i = 0; i < 12; i++)
        {
            if(i%5 != 0)
            {
                Vector2 position = new Vector2((parentTransform.position.x - 30f) + (i * 5), -11);
                Terrain terrain = Instantiate(TerrainPrefab, position, Quaternion.identity).GetComponent<Terrain>();
                terrain.transform.SetParent(terrainContainer.transform);
           
                terrains.Add(terrain);
            }
        }

        return terrains;
    }
}
