using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FixVector4))]
public class FixFixVector4Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty xProp = property.FindPropertyRelative("x").FindPropertyRelative("m_rawValue");
        SerializedProperty yProp = property.FindPropertyRelative("y").FindPropertyRelative("m_rawValue");
        SerializedProperty zProp = property.FindPropertyRelative("z").FindPropertyRelative("m_rawValue");
        SerializedProperty wProp = property.FindPropertyRelative("w").FindPropertyRelative("m_rawValue");

        Fix64 xVal;
        Fix64 yVal;
        Fix64 zVal;
        Fix64 wVal;
        xVal.m_rawValue = xProp.longValue;
        yVal.m_rawValue = yProp.longValue;
        zVal.m_rawValue = zProp.longValue;
        wVal.m_rawValue = wProp.longValue;

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

            xProp.longValue = newFixVec.x.m_rawValue;
            yProp.longValue = newFixVec.y.m_rawValue;
            zProp.longValue = newFixVec.z.m_rawValue;
            wProp.longValue = newFixVec.w.m_rawValue;
        }


        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector4, label);
    }
}
