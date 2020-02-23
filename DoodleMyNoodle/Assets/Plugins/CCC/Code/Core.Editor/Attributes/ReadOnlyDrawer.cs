using UnityEngine;
using UnityEditor;

namespace CCC.InspectorDisplay
{
    [CustomPropertyDrawer(typeof(ReadOnlyAlwaysAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUI.GetPropertyHeight(property, label, property.hasVisibleChildren);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            GUI.enabled = false;
            EditorGUI.PropertyField(position, property, label, property.hasVisibleChildren);
            GUI.enabled = true;
        }
    }
}