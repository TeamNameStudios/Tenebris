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

        if (GUILayout.Button("PLAY EFFECT"))
        {
            audioSystem.PlayClip(audioSystem.SoundToTest);
        }

        if (GUILayout.Button("PLAY MUSIC"))
        {
            audioSystem.PlayMusic(audioSystem.MusicToTest);
        }

        if (GUILayout.Button("STOP MUSIC"))
        {
            audioSystem.StopMusic(true);
        }
    }
}
