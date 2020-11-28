using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorX;
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
        SerializedProperty gameActionProperty = serializedObject.FindProperty("Value");
        EditorGUILayoutX.SearchablePopupString(gameActionProperty, label, s_availableTypeNames);

        SerializedProperty animationConditionProperty = serializedObject.FindProperty("PlayAnimation");
        EditorGUILayout.PropertyField(animationConditionProperty);

        if (animationConditionProperty.boolValue)
        {
            SerializedProperty animationProperty = serializedObject.FindProperty("Animation");
            EditorGUILayout.PropertyField(animationProperty);
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}