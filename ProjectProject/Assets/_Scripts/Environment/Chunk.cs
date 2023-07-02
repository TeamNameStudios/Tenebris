using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MapMover
{
    const float chunkSize = 60f;

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

    [SerializeField]
    private Shadow shadow;

    public void Setup(List<Terrain> terrains, List<Platform> platforms)
    {
        EndOfChunk = transform.position.x +( chunkSize / 2);
        Terrains = terrains;
        Platforms = platforms;
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
        if(EndOfChunk < -60f)
        {
            //Reset();
            ChunkGenerator.Instance.GenerateChunk(this,false);
        }
    }

    public void SetPreviousChunk(Chunk previous)
    {
        previousChunk = previous;
    }
}
