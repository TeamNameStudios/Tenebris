using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(AudioSystem))]
public class EditorTestSound : Editor
{
    AudioSystem audioSystem;

    private void Awake()
    {
        audioSystem = (AudioSystem)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("TEST SOUND"))
        {
            audioSystem.PlayClip(audioSystem.SoundToTest);
        }
    }
}
