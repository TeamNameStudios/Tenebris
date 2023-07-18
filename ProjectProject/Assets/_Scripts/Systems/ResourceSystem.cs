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
    #region Platform
        public List<ScriptablePlatform> Platforms { get; private set; }
        private Dictionary<PlatformType, ScriptablePlatform> PlatformsDict;
    #endregion

    #region Cultist
        public List<ScriptableCultist> Cultists { get; private set; }
        private Dictionary<CultistType, ScriptableCultist> CultistsDict;
    #endregion
    protected override void Awake()
    {
        base.Awake();
        AssembleResources();
    }

    private void AssembleResources()
    {
        Platforms = Resources.LoadAll<ScriptablePlatform>("Platforms").ToList();
        PlatformsDict = Platforms.ToDictionary(platform =>  platform.Type, platform => platform);

        Cultists = Resources.LoadAll<ScriptableCultist>("Cultists").ToList();
        CultistsDict = Cultists.ToDictionary(cultist => cultist.Type, cultist => cultist);
    }

    public ScriptablePlatform GetPlatform(PlatformType t) => PlatformsDict[t];
    public ScriptablePlatform GetRandomPlayer() => Platforms[Random.Range(0, Platforms.Count)];

    public ScriptableCultist GetCultist(CultistType t) => CultistsDict[t];
    public ScriptableCultist GetRandomCultist() => Cultists[Random.Range(0, Cultists.Count)];

}