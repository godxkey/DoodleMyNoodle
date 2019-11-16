using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FixVector2))]
public class FixFixVector2Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty xProp = property.FindPropertyRelative(nameof(FixVector2.x)).FindPropertyRelative(nameof(Fix64.RawValue));
        SerializedProperty yProp = property.FindPropertyRelative(nameof(FixVector2.y)).FindPropertyRelative(nameof(Fix64.RawValue));

        Fix64 xVal;
        Fix64 yVal;
        xVal.RawValue = xProp.longValue;
        yVal.RawValue = yProp.longValue;

        FixVector2 oldFixVec = new FixVector2(xVal, yVal);

        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Editor Field
        Vector2 oldVec = oldFixVec.ToUnityVec();
        Vector2 newVec = EditorGUI.Vector2Field(position, label, oldVec);

        // Change ?
        if (oldVec != newVec)
        {
            FixVector2 newFixVec = newVec.ToFixVec();

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
