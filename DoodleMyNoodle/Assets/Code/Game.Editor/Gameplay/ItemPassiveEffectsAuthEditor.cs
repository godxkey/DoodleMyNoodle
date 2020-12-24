using System;
using System.Linq;
using UnityEditor;
using UnityEditorX;
using UnityEngine;
using UnityEngineX;

[CustomEditor(typeof(ItemPassiveEffectsAuth))]
public class ItemPassiveEffectsAuthEditor : Editor
{
    private static string[] s_availableTypeNames;
    private static Type[] s_availableTypes;

    private int _listSize = 0;

    private void OnEnable()
    {
        if (s_availableTypes == null)
        {
            s_availableTypes = TypeUtility.GetTypesDerivedFrom(typeof(ItemPassiveEffect)).ToArray();
            s_availableTypeNames = s_availableTypes.Select(t => t.Name).ToArray();
        }
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();

        GUIContent label = EditorGUIUtilityX.TempContent("Item Passive Effect Type");

        SerializedProperty Values = serializedObject.FindProperty("Values");

        _listSize = Values.arraySize;
        _listSize = EditorGUILayout.IntField("List Size", _listSize);
        if (_listSize != Values.arraySize)
        {
            while (_listSize > Values.arraySize)
            {
                Values.InsertArrayElementAtIndex(Values.arraySize);
            }
            while (_listSize < Values.arraySize)
            {
                Values.DeleteArrayElementAtIndex(Values.arraySize - 1);
            }
        }

        for (int i = 0; i < Values.arraySize; i++)
        {
            EditorGUILayoutX.SearchablePopupString(Values.GetArrayElementAtIndex(i), label, s_availableTypeNames);
        }

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }
    }
}