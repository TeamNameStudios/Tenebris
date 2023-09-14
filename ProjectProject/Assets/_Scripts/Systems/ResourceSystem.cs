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
    #region DialogueLines
    public List<ScriptableTutorialDialogue> DialogueLines { get; private set; }

    private Dictionary<string, ScriptableTutorialDialogue> DialogueDict = new Dictionary<string, ScriptableTutorialDialogue>();
    #endregion

    #region PowerUp
    public List<ScriptablePowerUp> PowerUps { get; private set; }
    public List<ScriptablePowerUp> InitPowerUps { get; private set; }
    #endregion

    #region Level
    public List<ScriptableLevelChunk> LevelChunks { get; private set; }
    private Dictionary<LevelID, ScriptableLevelChunk> LevelChunksDict = new Dictionary<LevelID, ScriptableLevelChunk>();
      
    private List<LevelID> easyLevels = new List<LevelID>();
    private List<LevelID> mediumLevels = new List<LevelID>();
    private List<LevelID> hardLevels = new List<LevelID>();
    private List<LevelID> insaneLevels = new List<LevelID>();
    private Dictionary<LevelDifficulty, List<LevelID>> levelDictByDifficulty = new Dictionary<LevelDifficulty, List<LevelID>>();
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
        DialogueLines = Resources.LoadAll<ScriptableTutorialDialogue>("DialogueLines").ToList();
        LevelChunksDict = LevelChunks.ToDictionary(levelChunk => levelChunk.ID, levelChunk => levelChunk);
        DialogueDict = DialogueLines.ToDictionary(dialogueLines => dialogueLines.dialogueName, dialogueName => dialogueName);

        levelDictByDifficulty.Add(LevelDifficulty.EASY, easyLevels);
        levelDictByDifficulty.Add(LevelDifficulty.MEDIUM, mediumLevels);
        levelDictByDifficulty.Add(LevelDifficulty.HARD, hardLevels);
        levelDictByDifficulty.Add(LevelDifficulty.INSANITY, insaneLevels);
    }

    public ScriptableTutorialDialogue GetDialogueLines(string name) => DialogueDict[name];
    
    public ScriptableLevelChunk GetLevelChunk(LevelID t) => LevelChunksDict[t];

    public void SetLevelScriptable()
    {
        for(int i=0;i< LevelChunks.Count; i++)
        {
            LevelChunks[i].InGameProbability = LevelChunks[i].OriginalProbability;
            LevelChunks[i].Probability = LevelChunks[i].OriginalProbability;

            LevelChunks[i].InGamePossibleNeighbour = new List<LevelID>(LevelChunks[i].PossibleNeighbour);

            levelDictByDifficulty[LevelChunks[i].Difficulty].Add(LevelChunks[i].ID);

            if (LevelChunks[i].HasManifestation)
            {
                manifestationLevels.Add(LevelChunks[i].ID);
            }

            if (LevelChunks[i].HasCollectible)
            {
                collectibleLevels.Add(LevelChunks[i].ID);
            }
        }
    }

    public void ChangeBaseProbability(LevelDifficulty difficulty, float newProbability)
    {
        for(int i=0;i< levelDictByDifficulty[difficulty].Count; i++)
        {
            LevelID level = levelDictByDifficulty[difficulty][i];
            LevelChunksDict[level].InGameProbability += newProbability;
            LevelChunksDict[level].Probability += newProbability;
        }
    }

    public void RemoveNeighbourByDifficulty(LevelDifficulty difficulty)
    {
        for (int i = 0; i< LevelChunks.Count; i++) {
            for (int x = 0; x < levelDictByDifficulty[difficulty].Count; x++)
            {
                LevelChunks[i].InGamePossibleNeighbour.Remove(levelDictByDifficulty[difficulty][x]);
            }
        }
    }

    public void RemoveNeighbourByCollectible()
    {

        for (int i = 0; i < LevelChunks.Count; i++)
        {
            for (int x = 0; x < collectibleLevels.Count; x++)
            {
                LevelChunks[i].InGamePossibleNeighbour.Remove(collectibleLevels[x]);
            }
        }
    }

    public void RemoveNeighbourByManifestation()
    {
        for (int i = 0; i < LevelChunks.Count; i++)
        {
            for (int x = 0; x < manifestationLevels.Count; x++)
            {
                LevelChunks[i].InGamePossibleNeighbour.Remove(manifestationLevels[x]);
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
        for(int i=0;i<PowerUps.Count;i++) 
        {
            if (!powerUpEnums.Contains(PowerUps[i].ID)){
                powerUpEnums.Add(PowerUps[i].ID);
            }
            
        }
        return powerUpEnums;
    }
}