using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Platform", menuName = "Create Platform")]
public class ScriptablePlatform : ScriptableObject
{
    public PlatformType Type;
    public PlatformSize Size;
    public GameObject PlatformPrefab;
}

public enum PlatformSize
{
   SMALL,
   MEDIUM,
   LARGE
}

public enum PlatformType
{
    NORMAL,
    FLAOTING,
    LIFT
}