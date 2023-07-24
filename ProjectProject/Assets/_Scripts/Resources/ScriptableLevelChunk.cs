using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Level Chunk", menuName = "Create  Level Chunk")]
public class ScriptableLevelChunk : ScriptableObject
{
    public LevelID ID;
    public LevelDifficulty Difficulty;
    public GameObject LevelPrefab;
    public float BaseProbability;
    public float Probability;
    public List<LevelID> PossibleNeighbour;
}

public enum LevelID
{
    QUADRATO,
    CERCHIO,
    TRIANGOLO
}
public  enum LevelDifficulty
{
    DEMO,
    EASY,
    MEDIUM,
    HARD,
    INSANITY,
}