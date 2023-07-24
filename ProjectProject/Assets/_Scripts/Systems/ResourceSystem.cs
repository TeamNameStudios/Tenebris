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
        private Dictionary<LevelID, ScriptableLevelChunk> LevelChunksict;
    #endregion
    protected override void Awake()
    {
        base.Awake();
        AssembleResources();
    }

    private void AssembleResources()
    {
        LevelChunks = Resources.LoadAll<ScriptableLevelChunk>("LevelChunks").ToList();
        LevelChunksict = LevelChunks.ToDictionary(levelChunk => levelChunk.ID, levelChunk => levelChunk);
    }

    public ScriptableLevelChunk GetLevelChunk(LevelID t) => LevelChunksict[t];

}