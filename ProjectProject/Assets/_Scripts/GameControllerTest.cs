using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerTest : MonoBehaviour
{
    public Transform chunk_ONE;
    public Transform chunk_TWO;

    public LevelID levelID_ONE;
    public LevelID levelID_TWO;

    private void Start()
    {
        LevelAssembler.Instance.CreateLevelChunk(levelID_ONE, chunk_ONE);
        LevelAssembler.Instance.CreateLevelChunk(levelID_TWO, chunk_TWO);
    }


    private void Update()
    {
        if(GameController.Instance.State == GameState.STARTING)
        {
            EventManager<bool>.Instance.TriggerEvent("onMapGenerated", true);
        }
    }
}
