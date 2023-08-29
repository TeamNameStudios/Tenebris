using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        if (transform.position.x <= -(chunkSize + chunkOffset))
        {
            Vector3 pos = transform.position;
            pos.x += (chunkSize * (EnvironmentController.Instance.numberOfChunk));

            //float lastChunkPos = EnvironmentController.Instance.FindLastChunkPosition(); //TEST JACK
            //pos.x = lastChunkPos + chunkSize;

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
        //EnvironmentController.Instance.ManageList(); //TEST JACK
    }

    public void CheckDistance()
    {
        int index = EnvironmentController.Instance.listChunk.IndexOf(this);
        if (index != 0)
        {
            Vector2 pos = transform.position;
            float distance = Vector2.Distance(transform.position, EnvironmentController.Instance.listChunk[index - 1].transform.position);
            Debug.Log(Vector2.Distance(transform.position, EnvironmentController.Instance.listChunk[index - 1].transform.position));
            if (distance != 66)
            {
                float offset = Mathf.Abs(distance - chunkSize);
                Debug.Log(offset);
                pos.x += offset;
                transform.position = pos;
                Debug.Log(Vector2.Distance(transform.position, EnvironmentController.Instance.listChunk[index - 1].transform.position));
            }
        }
    }
}
