using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FixVector2))]
public class FixFixVector2Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty xProp = property.FindPropertyRelative("x").FindPropertyRelative("m_rawValue");
        SerializedProperty yProp = property.FindPropertyRelative("y").FindPropertyRelative("m_rawValue");

        Fix64 xVal;
        Fix64 yVal;
        xVal.m_rawValue = xProp.longValue;
        yVal.m_rawValue = yProp.longValue;

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

            xProp.longValue = newFixVec.x.m_rawValue;
            yProp.longValue = newFixVec.y.m_rawValue;
        }


        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector2, label);
    }
}
