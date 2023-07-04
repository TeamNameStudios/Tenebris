using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Chunk : MapMover
{
    const float chunkSize = 60f;

    [SerializeField]
    private Chunk previousChunk;

    private List<Terrain> terrains = new List<Terrain>();
    public List<Terrain> Terrains
    {
        get { return terrains; }
        set { terrains = value; }
    }

    public GameObject terrainContainer;

    public GameObject platformContainer;

    private List<Platform> platforms = new List<Platform>();
    public List<Platform> Platforms
    {
        get { return platforms; }
        set { platforms = value; }
    }
    
    [SerializeField]
    private float EndOfChunk;
    
    [SerializeField]
    private int IndexOfChunk;

    [SerializeField]
    private Shadow shadow;

    public bool isGenerated;

    public void Setup(List<Terrain> terrains, List<Platform> platforms, GameObject _terrainContainer, GameObject _platformContainer)
    {
        EndOfChunk = transform.position.x +( chunkSize / 2);
        Terrains = terrains;
        Platforms = platforms;

        terrainContainer = _terrainContainer;
        platformContainer = _platformContainer;
    }

    public void Update()
    {
        Vector2 pos = transform.position;
        
        if (pos.x < -60f)
        {
            pos.x += 180f;
        }
        if (pos.x > 60f)
        {
            pos.x -= 180f;
            isGenerated = false;
        }
        
        transform.position = pos;
        EndOfChunk = transform.position.x + (chunkSize / 2);

        if (EndOfChunk < -60f)
        {
            ResetChunk(this);
        }
    }

    public void SetPreviousChunk(Chunk previous)
    {
        previousChunk = previous;
    }

    public void ResetChunk(Chunk chunk)
    {
        chunk.terrains.Clear();
        chunk.platforms.Clear();

        terrainContainer.transform.DestroyChildren();

        platformContainer.transform.DestroyChildren();

        chunk.GetComponentInChildren<Tilemap>().ClearAllTiles();
        
        if (!isGenerated)
        {
            ChunkGenerator.Instance.GenerateChunk(this);  //whi it is being called every frame even if isGenerated is false ????
            isGenerated = true;
        }
    }
}
