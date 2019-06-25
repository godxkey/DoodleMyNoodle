using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SimComponentView), editorForChildClasses: true)]
public class SimComponentViewEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SimComponentView view = (SimComponentView)target;
        if (Application.isPlaying && Simulation.instance != null && view.attachedToSim)
        {
            view.UpdateSerializedDataFromSim();
        }
    }
}
