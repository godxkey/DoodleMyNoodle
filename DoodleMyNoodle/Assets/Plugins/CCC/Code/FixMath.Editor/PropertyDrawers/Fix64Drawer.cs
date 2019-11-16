using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Fix64))]
public class Fix64Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty valueProperty = property.FindPropertyRelative(nameof(Fix64.RawValue));
        long valueRaw = valueProperty.longValue;

        Fix64 value;
        value.RawValue = valueRaw;

        float floatValue = (float)value;

        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Editor Field
        float newFloatValue = EditorGUI.FloatField(position, label, floatValue);

        // Change ?
        if(newFloatValue != floatValue)
        {
            Fix64 newValue = (Fix64)newFloatValue;
            valueProperty.longValue = newValue.RawValue;
        }


        EditorGUI.EndProperty();
    }
}
