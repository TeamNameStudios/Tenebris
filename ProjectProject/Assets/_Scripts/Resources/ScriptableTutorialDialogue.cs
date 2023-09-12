using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialogue Lines", menuName = "Create Dialogue Lines")]
public class ScriptableTutorialDialogue : ScriptableObject
{
    public string dialogueName;
    public List<string> lines;
    public Vector3 position;
}
