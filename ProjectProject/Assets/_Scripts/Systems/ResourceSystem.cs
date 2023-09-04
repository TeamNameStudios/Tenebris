using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

/// <summary>
/// One repository for all scriptable objects. Create your query methods here to keep your business logic clean.
/// I make this a MonoBehaviour as sometimes I add some debug/development references in the editor.
/// If you don't feel free to make this a standard class
/// </summary>
public class ResourceSystem : StaticInstance<ResourceSystem>
{
    #region DialogueLines
    public List<TutorialDialogueScriptable> DialogueLines { get; private set; }

    public Dictionary<string, TutorialDialogueScriptable> DialogueDict = new Dictionary<string, TutorialDialogueScriptable>();
    #endregion

    #region PowerUp
    public List<ScriptablePowerUp> PowerUps { get; private set; }
    public List<ScriptablePowerUp> InitPowerUps { get; private set; }
    #endregion

    #region Level
    [SerializeField] public List<ScriptableLevelChunk> LevelChunks { get; private set; }
    [SerializeField]
    public Dictionary<LevelID, ScriptableLevelChunk> LevelChunksDict = new Dictionary<LevelID, ScriptableLevelChunk>();

       
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
        PowerUps = Resources.LoadAll<ScriptablePowerUp>("PowerUps").ToList();
        LevelChunks = Resources.LoadAll<ScriptableLevelChunk>("LevelChunks").ToList();
        DialogueLines = Resources.LoadAll<TutorialDialogueScriptable>("DialogueLines").ToList();
        LevelChunksDict = LevelChunks.ToDictionary(levelChunk => levelChunk.ID, levelChunk => levelChunk);
        DialogueDict = DialogueLines.ToDictionary(dialogueLines => dialogueLines.dialogueName, dialogueName => dialogueName);

        levelDictByDifficulty.Add(LevelDifficulty.EASY, easyLevels);
        levelDictByDifficulty.Add(LevelDifficulty.MEDIUM, mediumLevels);
        levelDictByDifficulty.Add(LevelDifficulty.HARD, hardLevels);
        levelDictByDifficulty.Add(LevelDifficulty.INSANITY, insaneLevels);
    }

    public TutorialDialogueScriptable GetDialogueLines(string name) => DialogueDict[name];
    
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

    // TO DO: REFACTOR W/ FIND
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

    public ScriptablePowerUp GetPowerUp(PowerUpEnum id, int Level) {
            return PowerUps.Find((powerUp) =>
            {
                return powerUp.ID == id && powerUp.Level == Level;
            });
    }

    public List<PowerUpEnum> GetInitsPowerUp()
    {
       List<PowerUpEnum> powerUpEnums = new List<PowerUpEnum>();
        foreach (var powerUp in PowerUps)
        {
            if (!powerUpEnums.Contains(powerUp.ID)){
                powerUpEnums.Add(powerUp.ID);
            }
            
        }
        return powerUpEnums;
    }
}