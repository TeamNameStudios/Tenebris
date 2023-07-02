using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class ChunkGenerator : Singleton<ChunkGenerator>
{
    [SerializeField]
    public GameObject ChunksContainer;
    public GameObject ChunkPrefab;

    private float chunkSize = 60;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Chunk GenerateChunk(int index)
    {
        float EndOfChunk = Vector3.right.x * index * chunkSize/2;
        float xPos = 0;
        if (index != 0) {
            xPos = index * chunkSize;
        }
        Vector2 position = new Vector2(xPos,0);
        Chunk chunk = Instantiate(ChunkPrefab, Vector2.zero, Quaternion.identity).GetComponent<Chunk>();
        chunk.transform.position = position;
        chunk.transform.SetParent(ChunksContainer.transform);
        chunk.Setup(index, chunkSize);
        return chunk;
    }
}
