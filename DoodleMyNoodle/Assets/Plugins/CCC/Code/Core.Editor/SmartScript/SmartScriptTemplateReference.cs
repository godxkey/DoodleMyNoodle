using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorX;
using UnityEngine;

[CreateAssetMenu(menuName = "CCC/Script Template Link", fileName = "_ScriptTemplate")]
public class SmartScriptTemplateReference : ScriptableObject
{
    public MonoScript TemplateScript;
}

[CustomEditor(typeof(SmartScriptTemplateReference))]
public class SmartScriptTemplateReferenceEditor : Editor
{
    private DefaultSmartScriptResolver _defaultSmartScriptResolver = new DefaultSmartScriptResolver();

    [OnOpenAsset(1)]
    public static bool OnOpenAsset(int instanceID, int line)
    {
        var templateLink = EditorUtility.InstanceIDToObject(instanceID) as SmartScriptTemplateReference;

        if (templateLink != null && templateLink.TemplateScript)
        {
            return AssetDatabase.OpenAsset(templateLink.TemplateScript);
        }

        return false; // we did not handle the open
    }

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox($"Pick a script that contains a class inheriting of {nameof(ScriptTemplate)}.", MessageType.Info);
        
        base.OnInspectorGUI();

        serializedObject.Update();
        var castedTarget = (SmartScriptTemplateReference)target;

        MonoScript script = castedTarget.TemplateScript as MonoScript;
        if (script != null)
        {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);

            _defaultSmartScriptResolver.GetNewScriptContent(script, out string content, out string defaultName);

            EditorGUILayout.LabelField($"{defaultName}.cs");
            EditorGUILayout.TextArea(content);
        }

        serializedObject.ApplyModifiedProperties();
    }
}
