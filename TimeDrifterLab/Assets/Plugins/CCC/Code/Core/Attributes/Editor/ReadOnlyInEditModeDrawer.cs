using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(ReadOnlyInEditMode))]
public class ReadOnlyInEditModeDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, (attribute as ReadOnlyInEditMode).forwardToChildren);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!Application.isPlaying)
            GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, (attribute as ReadOnlyInEditMode).forwardToChildren);
        GUI.enabled = true;
    }
}
