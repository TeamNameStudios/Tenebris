using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentController : Singleton<EnvironmentController>
{
    [SerializeField]
    public int numberOfChunk;
    [SerializeField]
    public List<Chunk> chunks;

    [SerializeField]
    public Player player;
    // Start is called before the first frame update
    void Start()
    { 
        for (int i = 0; i < numberOfChunk; i++)
        {
            AddChunk(i);  
        }
    }

    private void AddChunk(int index)
    {
        Chunk generatedChunk = ChunkGenerator.Instance.GenerateChunk(index);
        chunks.Add(generatedChunk);
    }

}
