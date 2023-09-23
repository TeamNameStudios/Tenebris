using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Chunk", menuName = "Create  Level Chunk")]
public class ScriptableLevelChunk : ScriptableObject
{
    public LevelID ID;
    public LevelDifficulty Difficulty;
    public GameObject LevelPrefab;
    public float OriginalProbability;
    public float InGameProbability;
    public float Probability;
    public List<LevelID> PossibleNeighbour;
    public List<LevelID> InGamePossibleNeighbour;

    public void AutoPopulate()
    {
        foreach (ScriptableLevelChunk level in ResourceSystem.Instance.LevelChunks)
        {
            PossibleNeighbour.Add(level.ID);
        }
    }

    public void ClearList()
    {
        PossibleNeighbour.Clear();
    }
}

public  enum LevelDifficulty
{
    EASY,
    MEDIUM,
    HARD,
    INSANITY,
}