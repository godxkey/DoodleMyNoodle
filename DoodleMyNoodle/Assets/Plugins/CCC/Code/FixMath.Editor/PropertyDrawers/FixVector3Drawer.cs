using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FixVector3))]
public class FixFixVector3Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty xProp = property.FindPropertyRelative("x").FindPropertyRelative("m_rawValue");
        SerializedProperty yProp = property.FindPropertyRelative("y").FindPropertyRelative("m_rawValue");
        SerializedProperty zProp = property.FindPropertyRelative("z").FindPropertyRelative("m_rawValue");

        Fix64 xVal;
        Fix64 yVal;
        Fix64 zVal;
        xVal.m_rawValue = xProp.longValue;
        yVal.m_rawValue = yProp.longValue;
        zVal.m_rawValue = zProp.longValue;

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

            xProp.longValue = newFixVec.x.m_rawValue;
            yProp.longValue = newFixVec.y.m_rawValue;
            zProp.longValue = newFixVec.z.m_rawValue;
        }


        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3, label);
    }
}
