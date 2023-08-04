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
    #region Level
    public List<ScriptableLevelChunk> LevelChunks { get; private set; }
    [SerializeField] public Dictionary<LevelID, ScriptableLevelChunk> LevelChunksDict = new Dictionary<LevelID, ScriptableLevelChunk>();
    #endregion

    #region PowerUp
    public List<ScriptablePowerUp> PowerUps { get; private set; }
    public List<ScriptablePowerUp> InitPowerUps { get; private set; }
    #endregion
    protected override void Awake()
    {
        base.Awake();
        AssembleResources();
        ResetLevelProbability();
    }

    private void AssembleResources()
    {
        LevelChunks = Resources.LoadAll<ScriptableLevelChunk>("LevelChunks").ToList();
        LevelChunksDict = LevelChunks.ToDictionary(levelChunk => levelChunk.ID, levelChunk => levelChunk);
        PowerUps = Resources.LoadAll<ScriptablePowerUp>("PowerUps").ToList();
    }

    public ScriptableLevelChunk GetLevelChunk(LevelID t) => LevelChunksDict[t];

    public void ResetLevelProbability()
    {
        foreach (ScriptableLevelChunk level in LevelChunks)
        {
            level.InGameProbability = level.OriginalProbability;
            level.Probability = level.OriginalProbability;
        }
    }

    public void ChangeBaseProbability(LevelDifficulty difficulty, float newProbability)
    {
        for (int i = 0; i < LevelChunks.Count; i++)
        {
            if (LevelChunks[i].Difficulty == difficulty)
            {
                LevelChunks[i].InGameProbability += newProbability;
                LevelChunks[i].Probability += newProbability;
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