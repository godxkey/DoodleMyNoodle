using CCC.Editor;
using Unity.Properties;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FixVector4))]
public class FixFixVector4Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty xProp = property.FindPropertyRelative(nameof(FixVector4.x)).FindPropertyRelative(nameof(Fix64.RawValue));
        SerializedProperty yProp = property.FindPropertyRelative(nameof(FixVector4.y)).FindPropertyRelative(nameof(Fix64.RawValue));
        SerializedProperty zProp = property.FindPropertyRelative(nameof(FixVector4.z)).FindPropertyRelative(nameof(Fix64.RawValue));
        SerializedProperty wProp = property.FindPropertyRelative(nameof(FixVector4.w)).FindPropertyRelative(nameof(Fix64.RawValue));

        Fix64 xVal;
        Fix64 yVal;
        Fix64 zVal;
        Fix64 wVal;
        xVal.RawValue = xProp.longValue;
        yVal.RawValue = yProp.longValue;
        zVal.RawValue = zProp.longValue;
        wVal.RawValue = wProp.longValue;

        FixVector4 oldFixVec = new FixVector4(xVal, yVal, zVal, wVal);

        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Editor Field
        Vector4 oldVec = oldFixVec.ToUnityVec();
        Vector4 newVec = EditorGUI.Vector4Field(position, label, oldVec);

        // Change ?
        if (oldVec != newVec)
        {
            FixVector4 newFixVec = newVec.ToFixVec();

            xProp.longValue = newFixVec.x.RawValue;
            yProp.longValue = newFixVec.y.RawValue;
            zProp.longValue = newFixVec.z.RawValue;
            wProp.longValue = newFixVec.w.RawValue;
        }


        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector4, label);
    }
}


[CustomEntityPropertyDrawer]
public class FixVector4EntityDrawer : IMGUIAdapter,
        IVisitAdapter<FixVector4>
{
    VisitStatus IVisitAdapter<FixVector4>.Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref FixVector4 value, ref ChangeTracker changeTracker)
    {
        DoField(property, ref container, ref value, ref changeTracker, (label, val) =>
        {
            var newValue = EditorGUILayout.Vector4Field(label, val.ToUnityVec());

            return Application.isPlaying ? val : newValue.ToFixVec(); // we don't support runtime modif
        });

        return VisitStatus.Handled;
    }
}