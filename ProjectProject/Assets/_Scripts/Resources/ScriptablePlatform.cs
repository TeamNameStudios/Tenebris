using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Platform", menuName = "Create Platform")]
public class ScriptablePlatform : ScriptableObject
{
    public PlatformType Type;
    public GameObject PlatformPrefab;
}

public enum PlatformType
{
   SMALL,
   MEDIUM,
   LARGE
}