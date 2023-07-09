using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ChunkGenerator : Singleton<ChunkGenerator>
{
    [SerializeField]
    public GameObject ChunksContainer;
    public GameObject ChunkPrefab;
    public GameObject HookablePrefab;
    [SerializeField]
    private float chunkSize = 60;
   
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public Chunk CreateChunk(int index)
    {
        float xPos = 0;
        if (index != 0)
        {
            xPos = index * chunkSize;
        }
        Vector2 position = new Vector2(xPos, 0);
        Chunk chunk = Instantiate(ChunkPrefab, position, Quaternion.identity).GetComponent<Chunk>();
        chunk.transform.SetParent(ChunksContainer.transform);
        return chunk;
    }

    public Chunk GenerateChunk(Chunk chunk, bool initChunk = false) 
    {
        GameObject terrainContainer;
        List<Terrain> Terrains = TerrainGenerator.Instance.GenerateTerrains(chunk.transform, initChunk, out terrainContainer);

        GameObject platformContainer;
        List<Platform> Platforms = PlatformGenerator.Instance.GeneratePlatforms(chunk.transform, initChunk, out platformContainer);

        //chunk.Setup(Terrains, Platforms, terrainContainer, platformContainer);
     
        chunk.Setup(Platforms, platformContainer);

        //TileTerrainGeneration.Instance.GenerateTilemap(chunk.transform, EnvironmentController.Instance.tilemapDictionary[chunk]);

        return chunk;
    }

}
