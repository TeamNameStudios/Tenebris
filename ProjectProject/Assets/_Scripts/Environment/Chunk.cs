using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MapMover
{

    [SerializeField]
    private Chunk previousChunk;
    [SerializeField]
    private List<Terrain> Terrains = new List<Terrain>();
    [SerializeField]
    private List<Platform> Platforms = new List<Platform>();
    [SerializeField]
    private float EndOfChunk;
    [SerializeField]
    private int IndexOfChunk;

    public void Setup(int index, float chunkSize)
    {
        IndexOfChunk = index;
        EndOfChunk = transform.position.x +(chunkSize / 2);
        Terrains = TerrainGenerator.Instance.GenerateTerrains(transform);
        Platforms = PlatformGenerator.Instance.GeneratePlatforms(transform);
    }

    public void Setup(int index,float chunkSize, Chunk previous)
    {
        Setup(index, chunkSize);
        previousChunk = previous;
    }

    public void Update()
    {
        Vector2 pos = transform.position;
        if(pos.x < -60f)
        {
            pos.x += 180f;
        }
        if (pos.x > 60f)
        {
            pos.x -= 180f;
        }
        transform.position = pos;
    }
}
