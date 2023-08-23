using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ScriptableLevelChunk))]
public class AutoPopulateNeighbour : Editor
{
    ScriptableLevelChunk scriptableLevelChunk;

    private void Awake()
    {
        scriptableLevelChunk = (ScriptableLevelChunk)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Populate List"))
        {
            scriptableLevelChunk.AutoPopulate();
        }

        if (GUILayout.Button("Clear List"))
        {
            scriptableLevelChunk.ClearList();
        }
    }
}
