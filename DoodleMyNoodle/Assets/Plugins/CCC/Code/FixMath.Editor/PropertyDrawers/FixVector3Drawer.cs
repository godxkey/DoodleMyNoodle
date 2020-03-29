using CCC.Editor;
using Unity.Properties;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FixVector3))]
public class FixFixVector3Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty xProp = property.FindPropertyRelative(nameof(FixVector3.x)).FindPropertyRelative(nameof(Fix64.RawValue));
        SerializedProperty yProp = property.FindPropertyRelative(nameof(FixVector3.y)).FindPropertyRelative(nameof(Fix64.RawValue));
        SerializedProperty zProp = property.FindPropertyRelative(nameof(FixVector3.z)).FindPropertyRelative(nameof(Fix64.RawValue));

        Fix64 xVal;
        Fix64 yVal;
        Fix64 zVal;
        xVal.RawValue = xProp.longValue;
        yVal.RawValue = yProp.longValue;
        zVal.RawValue = zProp.longValue;

        FixVector3 oldFixVec = new FixVector3(xVal, yVal, zVal);        

        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Editor Field
        Vector3 oldVec = oldFixVec.ToUnityVec();
        Vector3 newVec =  EditorGUI.Vector3Field(position, label, oldVec);

        

        // Change ?
        if (oldVec != newVec)
        {
            FixVector3 newFixVec = newVec.ToFixVec();

            xProp.longValue = newFixVec.x.RawValue;
            yProp.longValue = newFixVec.y.RawValue;
            zProp.longValue = newFixVec.z.RawValue;
        }


        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3, label);
    }
}



[CustomEntityPropertyDrawer]
public class FixVector3EntityDrawer : IMGUIAdapter,
        IVisitAdapter<FixVector3>
{
    VisitStatus IVisitAdapter<FixVector3>.Visit<TProperty, TContainer>(IPropertyVisitor visitor, TProperty property, ref TContainer container, ref FixVector3 value, ref ChangeTracker changeTracker)
    {
        DoField(property, ref container, ref value, ref changeTracker, (label, val) =>
        {
            var newValue = EditorGUILayout.Vector3Field(label, val.ToUnityVec());

            return Application.isPlaying ? val : newValue.ToFixVec(); // we don't support runtime modif
        });

        return VisitStatus.Handled;
    }
}