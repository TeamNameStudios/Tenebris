using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(LevelNames))]
public class TestWriter : Editor
{
    LevelNames levelNames;
    string filePath = "Assets/_Scripts/";
    string fileName = "LevelID";

    private void OnEnable()
    {
        levelNames = (LevelNames)target;
        levelNames.levelNames = EditorMethods.ReadToEnum(filePath, fileName);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        filePath = EditorGUILayout.TextField("Path", filePath);
        fileName = EditorGUILayout.TextField("Name", fileName);

        if (GUILayout.Button("Save"))
        {
            EditorMethods.WriteToEnum(filePath, fileName, levelNames.levelNames);
        }

        if (GUILayout.Button("Read"))
        {
            levelNames.levelNames = EditorMethods.ReadToEnum(filePath, fileName);
        }
    }
}
