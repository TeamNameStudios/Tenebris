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
            transform.position = pos;
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
