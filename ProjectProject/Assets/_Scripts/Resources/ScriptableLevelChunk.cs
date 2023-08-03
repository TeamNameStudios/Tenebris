using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Chunk", menuName = "Create  Level Chunk")]
public class ScriptableLevelChunk : ScriptableObject
{
    public LevelID ID;
    public LevelDifficulty Difficulty;
    public bool HasManifestation;
    public bool HasCollectible;
    public GameObject LevelPrefab;
    public float OriginalProbability;
    public float InGameProbability;
    public float Probability;
    public List<LevelID> PossibleNeighbour;
}

public  enum LevelDifficulty
{
    DEMO,
    EASY,
    MEDIUM,
    HARD,
    INSANITY,
}