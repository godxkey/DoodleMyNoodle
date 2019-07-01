using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(AutoFetchAttribute))]
public class AutoFetchDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (!Application.isPlaying                                  // not in 'play' mode
            && property.serializedObject.targetObject is Component  // field is on component
            && property.objectReferenceValue == null                // reference is null
            && fieldInfo.FieldType.IsSubclassOf(typeof(Component))) // field is a component reference
        {
            // fetch!
            property.objectReferenceValue = ((Component)property.serializedObject.targetObject).GetComponent(fieldInfo.FieldType);
        }

        EditorGUI.PropertyField(position, property, label);
    }
}
