using UnityEditor;
using UnityEngine;
using CCC.Editor;
using Unity.Properties;

[CustomPropertyDrawer(typeof(fix))]
public class Fix64Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty valueProperty = property.FindPropertyRelative(nameof(fix.RawValue));
        long valueRaw = valueProperty.longValue;

        fix value;
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
            fix newValue = (fix)newFloatValue;
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
        IVisitAdapter<fix>
{
    VisitStatus IVisitAdapter<fix>.Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref fix value, ref ChangeTracker changeTracker)
    {
        DoField(property, ref container, ref value, ref changeTracker, (label, val) =>
        {
            var newValue = EditorGUILayout.FloatField(label, (float)val);

            return Application.isPlaying ? val : (fix)newValue; // we don't support runtime modif
        });

        return VisitStatus.Handled;
    }
}