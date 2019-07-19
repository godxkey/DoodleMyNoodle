using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimEntity))]
public class SimEntityEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (Application.isPlaying)
        {
            if(((SimEntity)target).isPartOfSimulation)
            {
                EditorGUILayout.HelpBox("Connected to simulation", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("Not connected to simulation", MessageType.Warning);
            }
        }
        base.OnInspectorGUI();
    }
}
