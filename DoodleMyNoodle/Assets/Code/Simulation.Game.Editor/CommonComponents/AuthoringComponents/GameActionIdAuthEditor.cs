using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngineX;

[CustomEditor(typeof(GameActionIdAuth))]
public class GameActionIdAuthEditor : Editor
{
    private static string[] s_availableTypeNames;
    private static Type[] s_availableTypes;

    private void OnEnable()
    {
        if (s_availableTypes == null)
        {
            s_availableTypes = TypeUtility.GetTypesDerivedFrom(typeof(GameAction)).ToArray();
            s_availableTypeNames = s_availableTypes.Select(t => t.Name).ToArray();
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        GUIContent label = EditorGUIUtilityX.TempContent("Game Action Type");
        SerializedProperty property = serializedObject.FindProperty("Value");
        EditorGUILayoutX.SearchablePopupString(property, label, s_availableTypeNames);

        if(EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}