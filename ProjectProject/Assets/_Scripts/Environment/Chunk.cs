using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Chunk : MapMover
{
    [SerializeField] private LevelID lastLevelID;
    [SerializeField] private LevelID currentLevelID;
    
    private void Start()
    {
        LevelAssembler.Instance.Setup(transform);
        lastLevelID = LevelID.CERCHIO;
    }

    public override void Update()
    {
        base.Update();

        if (transform.position.x <= -25)
        {
            Vector3 pos = transform.position;
            pos.x = 35;
            transform.position = pos;
            // here I should get the ID of the current level
            lastLevelID = currentLevelID;

            ResetChunk();
        }
        //else if (transform.position.x >= 45)
        //{
        //    Vector3 pos = transform.position;
        //    pos.x = -18;
        //    transform.position = pos;
        //    LevelAssembler.Instance.CreateLevelChunk(lastLevelID, transform, false);
        //}
    }

    private void CreateChunk()
    {
        currentLevelID = LevelAssembler.Instance.CreateChunk(transform);
    }

    private void ResetChunk()
    {

        transform.DestroyChildren();
        CreateChunk();
    }
}
