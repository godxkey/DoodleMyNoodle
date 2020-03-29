using UnityEditor;
using UnityEngine;
using CCC.Editor;
using Unity.Properties;

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

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return 16;
    }
}

[CustomEntityPropertyDrawer]
public class Fix64EntityDrawer : IMGUIAdapter,
        IVisitAdapter<Fix64>
{
    VisitStatus IVisitAdapter<Fix64>.Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref Fix64 value, ref ChangeTracker changeTracker)
    {
        DoField(property, ref container, ref value, ref changeTracker, (label, val) =>
        {
            var newValue = EditorGUILayout.FloatField(label, (float)val);

            return Application.isPlaying ? val : (Fix64)newValue; // we don't support runtime modif
        });

        return VisitStatus.Handled;
    }
}