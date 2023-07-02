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
    // Start is called before the first frame update
    void Start()
    { 
        for (int i = 0; i < numberOfChunk; i++)
        {
            AddChunk(i);  
        }
        chunks[0].SetPreviousChunk(chunks[chunks.Count - 1]);
    }

    private void AddChunk(int index)
    {

        Chunk generatedChunk = ChunkGenerator.Instance.CreateChunk(index);
        generatedChunk = ChunkGenerator.Instance.GenerateChunk(generatedChunk, index == 0);
        chunks.Add(generatedChunk);
        if(chunks.Count > 0) {
            generatedChunk.SetPreviousChunk(chunks[index]);
        }
    }

}
