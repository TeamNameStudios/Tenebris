using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Cultist", menuName = "Create Cultist")]
public class ScriptableCultist : ScriptableObject
{
    public CultistType Type;
    public GameObject CultistPrefab;

}

public enum CultistType 
{
    BASE
} 