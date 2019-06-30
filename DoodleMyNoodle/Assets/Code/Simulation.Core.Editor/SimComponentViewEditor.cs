using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SimComponentView), editorForChildClasses: true)]
public class SimComponentViewEditor : Editor
{

    public override void OnInspectorGUI()
    {
        SimComponentView view = (SimComponentView)target;
        bool playingAndAttached = Application.isPlaying && Simulation.instance != null && view.attachedToSim;

        if (playingAndAttached)
        {
            view.UpdateSerializedDataFromSim(); // read from sim
            EditorGUILayout.HelpBox("WARNING: Modifying a property will alter the simulation an cause a desynchronisation!", MessageType.Warning);
        }


        EditorGUI.BeginChangeCheck();

        base.OnInspectorGUI();

        if(EditorGUI.EndChangeCheck() && playingAndAttached)
        {
            view.UpdateSimFromSerializedData(); // write to sim
        }
    }
}
