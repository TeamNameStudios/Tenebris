using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    private Chunk previousChunk;

    private float endOfChunck;
    
    private List<Terrain> terrainList;
    private List<Platform> platformList;

    private int chunkIndex;
    public int ChunkIndex
    {
        get { return chunkIndex; }
    }

    private void Awake()
    {
        terrainList = new List<Terrain>();
        platformList = new List<Platform>();
    }
}
