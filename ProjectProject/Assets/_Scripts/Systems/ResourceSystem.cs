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
        [SerializeField] private Dictionary<LevelID, ScriptableLevelChunk> LevelChunksict = new Dictionary<LevelID, ScriptableLevelChunk>();
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
        LevelChunksict = LevelChunks.ToDictionary(levelChunk => levelChunk.ID, levelChunk => levelChunk);
    }

    public ScriptableLevelChunk GetLevelChunk(LevelID t) => LevelChunksict[t];

    public void ResetLevelProbability()
    {
        foreach (ScriptableLevelChunk level in LevelChunks)
        {
            level.Probability = level.BaseProbability;
        }
    }
}