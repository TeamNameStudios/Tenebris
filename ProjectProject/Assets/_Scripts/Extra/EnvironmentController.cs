//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Tilemaps;

//public class EnvironmentController : Singleton<EnvironmentController>
//{
//    [SerializeField]
//    public int numberOfChunk;
//    [SerializeField]
//    public List<Chunk> chunks;

//    public Dictionary<Chunk, Tilemap> tilemapDictionary = new Dictionary<Chunk, Tilemap>();


//    private void OnEnable()
//    {
//        EventManager<bool>.Instance.StartListening("onGameStartingState", GenerateMap);
//    }
//    private void OnDisable()
//    {
//        EventManager<bool>.Instance.StopListening("onGameStartingState", GenerateMap);
//    }
//    void Start()
//    {

//        //for (int i = 0; i < numberOfChunk; i++)
//        //{
//        //    AddChunk(i); 
//        //}
//        //chunks[0].SetPreviousChunk(chunks[chunks.Count - 1]);
//    }

//    private void AddChunk(int index)
//    {

//        Chunk generatedChunk = ChunkGenerator.Instance.CreateChunk(index);
//        Tilemap tilemap = TileTerrainGeneration.Instance.CreateTilemap(generatedChunk.transform);
//        tilemapDictionary.Add(generatedChunk, tilemap);
//        generatedChunk = ChunkGenerator.Instance.GenerateChunk(generatedChunk, index == 0);
//        //GameObject hookable = Instantiate(ChunkGenerator.Instance.HookablePrefab, new Vector2(60*index,0), Quaternion.identity);
//        //hookable.transform.SetParent(generatedChunk.transform);
//        chunks.Add(generatedChunk);
//        if(chunks.Count > 0) {
//            generatedChunk.SetPreviousChunk(chunks[index]);
//        }
//    }

//    public void GenerateMap(bool isStarted)
//    {
//        for (int i = 0; i < numberOfChunk; i++)
//        {
//            AddChunk(i);
//        }
//        chunks[0].SetPreviousChunk(chunks[chunks.Count - 1]);
//        EventManager<bool>.Instance.TriggerEvent("endedGeneratedMap",true);
//    }



//}