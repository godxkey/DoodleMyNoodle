using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ViewBindingDefinition))]
public class ViewBindingDefinitionEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();


        var viewBindingDefinition = target as ViewBindingDefinition;

        if (viewBindingDefinition.GetSimGameObject() == null)
        {
            EditorGUILayout.HelpBox("No sim gameobject found in children", MessageType.Error);
        }
        else if (!viewBindingDefinition.GetSimGameObject().GetComponent<SimAsset>())
        {
            EditorGUILayout.HelpBox("The sim gameobject needs a SimAssetId component", MessageType.Error);
        }
        
        if (viewBindingDefinition.GetViewGameObject() == null)
        {
            EditorGUILayout.HelpBox("No view gameobject found in children", MessageType.Error);
        }
    }
}
