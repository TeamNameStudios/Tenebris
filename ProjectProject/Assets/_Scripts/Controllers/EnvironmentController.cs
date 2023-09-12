using System.Collections.Generic;
using UnityEngine;


public class EnvironmentController : Singleton<EnvironmentController>
{
    [SerializeField]
    public int numberOfChunk;
    [SerializeField]
    GameObject ChunkContainer;
    [SerializeField]
    Chunk chunkPrefab;
    [SerializeField]
    public List<Chunk> listChunk = new List<Chunk>();
    [SerializeField]
    LevelID firstLevel;
    [SerializeField]
    LevelID firstLevelTutorial;

    protected override void Awake()
    {
        base.Awake();
        if (GameController.Instance.IsTutorial == 1)
        {
            firstLevel = LevelID.Tutorial_1;
        }
    }

    private void OnEnable()
    {
        EventManager<bool>.Instance.StartListening("onGameStartingState", GenerateChunk);
    }
    private void OnDisable()
    {
        EventManager<bool>.Instance.StopListening("onGameStartingState", GenerateChunk);
    }

    public void GenerateChunk(bool isStarted)
    {
        for (int i = 0; i < numberOfChunk; i++)
        {
            Chunk Chunk = Instantiate(chunkPrefab,new Vector3(i*chunkPrefab.chunkSize,0,0), Quaternion.identity);
            Chunk.name = "Chunk " + i;
            Chunk.transform.SetParent(ChunkContainer.transform);
            BuildChunk(Chunk, i == 0);
            listChunk.Add(Chunk); 
        }
        EventManager<bool>.Instance.TriggerEvent("onMapGenerated", true);
    }

    public void BuildChunk(Chunk Chunk,bool isFirst)
    {
        if (isFirst)
        {
            Chunk.BuildLevelById(firstLevel);
        }
        else
        {
            Chunk.BuildLevel();
        }
    }

    public float FindLastChunkPosition()
    {
        float position = listChunk[listChunk.Count - 1].transform.position.x;
        return position;
    }
}
