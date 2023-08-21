using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// One repository for all scriptable objects. Create your query methods here to keep your business logic clean.
/// I make this a MonoBehaviour as sometimes I add some debug/development references in the editor.
/// If you don't feel free to make this a standard class
/// </summary>
public class ResourceSystem : StaticInstance<ResourceSystem>
{
    #region Level
        public List<ScriptableLevelChunk> LevelChunks { get; private set; }
        [SerializeField] public Dictionary<LevelID, ScriptableLevelChunk> LevelChunksDict = new Dictionary<LevelID, ScriptableLevelChunk>();
       
    private List<LevelID> easyLevels = new List<LevelID>();
    private List<LevelID> mediumLevels = new List<LevelID>();
    private List<LevelID> hardLevels = new List<LevelID>();
    private List<LevelID> insaneLevels = new List<LevelID>();
    public Dictionary<LevelDifficulty, List<LevelID>> levelDictByDifficulty = new Dictionary<LevelDifficulty, List<LevelID>>();

    private List<LevelID> collectibleLevels = new List<LevelID>();
    private List<LevelID> manifestationLevels = new List<LevelID>();

    #endregion
    protected override void Awake()
    {
        base.Awake();
        AssembleResources();
        SetLevelScriptable();
    }

    private void AssembleResources()
    {
        LevelChunks = Resources.LoadAll<ScriptableLevelChunk>("LevelChunks").ToList();
        LevelChunksDict = LevelChunks.ToDictionary(levelChunk => levelChunk.ID, levelChunk => levelChunk);

        levelDictByDifficulty.Add(LevelDifficulty.EASY, easyLevels);
        levelDictByDifficulty.Add(LevelDifficulty.MEDIUM, mediumLevels);
        levelDictByDifficulty.Add(LevelDifficulty.HARD, hardLevels);
        levelDictByDifficulty.Add(LevelDifficulty.INSANITY, insaneLevels);
    }

    public ScriptableLevelChunk GetLevelChunk(LevelID t) => LevelChunksDict[t];

    public void SetLevelScriptable()
    {
        foreach (ScriptableLevelChunk level in LevelChunks)
        {
            level.InGameProbability = level.OriginalProbability;
            level.Probability = level.OriginalProbability;
            
            level.InGamePossibleNeighbour = new List<LevelID>(level.PossibleNeighbour);

            levelDictByDifficulty[level.Difficulty].Add(level.ID);

            if (level.HasManifestation)
            {
                manifestationLevels.Add(level.ID);
            }

            if (level.HasCollectible)
            {
                collectibleLevels.Add(level.ID);
            }
        }
    }

    public void ChangeBaseProbability(LevelDifficulty difficulty, float newProbability)
    {
        //for (int i = 0; i < LevelChunks.Count; i++)
        //{
        //    if (LevelChunks[i].Difficulty == difficulty)
        //    {
        //        LevelChunks[i].InGameProbability += newProbability;
        //        LevelChunks[i].Probability += newProbability;
        //    }
        //}

        foreach (LevelID level in levelDictByDifficulty[difficulty])
        {
            LevelChunksDict[level].InGameProbability += newProbability;
            LevelChunksDict[level].Probability += newProbability;
        }
    }

    public void RemoveNeighbourByDifficulty(LevelDifficulty difficulty)
    {
        foreach (ScriptableLevelChunk level in LevelChunks)
        {
           foreach (LevelID levelID in levelDictByDifficulty[difficulty])
           {
               level.InGamePossibleNeighbour.Remove(levelID);
           }
        }
    }

    public void RemoveNeighbourByCollectible()
    {
        foreach (ScriptableLevelChunk level in LevelChunks)
        {
            foreach (LevelID levelID in collectibleLevels)
            {
                level.InGamePossibleNeighbour.Remove(levelID);
            }
        }
    }

    public void RemoveNeighbourByManifestation()
    {
        foreach (ScriptableLevelChunk level in LevelChunks)
        {
            foreach (LevelID levelID in manifestationLevels)
            {
                level.InGamePossibleNeighbour.Remove(levelID);
            }
        }
    }
}