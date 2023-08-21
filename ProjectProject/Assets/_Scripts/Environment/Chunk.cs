using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Chunk : MapMover
{
    [SerializeField] 
    private LevelID currentLevelID;
    [SerializeField] 
    public float chunkSize;
    [SerializeField]
    public float chunkOffset;
    public override void Update()
    {
        base.Update();

        if (transform.position.x <= -(chunkSize+ chunkOffset))
        {
            Vector3 pos = transform.position;
            pos.x = (chunkSize * (EnvironmentController.Instance.numberOfChunk-1)) - chunkOffset;
            transform.position = pos;
            // here I should get the ID of the current level
      
            ResetChunk();
        }

    }

    public void BuildLevel()
    {
        currentLevelID = LevelAssembler.Instance.CreateChunk(transform);
    }
    public void BuildLevelById(LevelID ID)
    {
        LevelAssembler.Instance.CreateLevelChunk(ID , transform);
        currentLevelID = ID;
    }

    private void ResetChunk()
    {

        transform.DestroyChildren();
        BuildLevel();
    }
}
