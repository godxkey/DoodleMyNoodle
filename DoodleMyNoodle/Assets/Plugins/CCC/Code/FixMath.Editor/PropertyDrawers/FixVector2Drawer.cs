using CCC.Editor;
using Unity.Properties;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(fix2))]
public class FixVector2Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty xProp = property.FindPropertyRelative(nameof(fix2.x)).FindPropertyRelative(nameof(fix.RawValue));
        SerializedProperty yProp = property.FindPropertyRelative(nameof(fix2.y)).FindPropertyRelative(nameof(fix.RawValue));

        fix xVal;
        fix yVal;
        xVal.RawValue = xProp.longValue;
        yVal.RawValue = yProp.longValue;

        fix2 oldFixVec = new fix2(xVal, yVal);

        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Editor Field
        Vector2 oldVec = oldFixVec.ToUnityVec();
        Vector2 newVec = EditorGUI.Vector2Field(position, label, oldVec);

        // Change ?
        if (oldVec != newVec)
        {
            fix2 newFixVec = newVec.ToFixVec();

            xProp.longValue = newFixVec.x.RawValue;
            yProp.longValue = newFixVec.y.RawValue;
        }


        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector2, label);
    }
}


[CustomEntityPropertyDrawer]
public class FixVector2EntityDrawer : IMGUIAdapter,
        IVisitAdapter<fix2>
{
    VisitStatus IVisitAdapter<fix2>.Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref fix2 value, ref ChangeTracker changeTracker)
    {
        DoField(property, ref container, ref value, ref changeTracker, (label, val) =>
        {
            var newValue = EditorGUILayout.Vector2Field(label, val.ToUnityVec());

            return Application.isPlaying ? val : newValue.ToFixVec(); // we don't support runtime modif
        });

        return VisitStatus.Handled;
    }
}