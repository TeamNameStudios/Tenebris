using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameControllerTest : MonoBehaviour
{
    public Player player;
    public Transform chunk_ONE;
    public Transform chunk_TWO;

    public LevelID levelID_ONE;
    public LevelID levelID_TWO;

    private void Start()
    {
        Instantiate(player, new Vector2(0, 16), Quaternion.identity);

        LevelAssembler.Instance.CreateLevelChunk(levelID_ONE, chunk_ONE);
        LevelAssembler.Instance.CreateLevelChunk(levelID_TWO, chunk_TWO);
    }
}
