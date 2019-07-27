using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FixVector3))]
public class FixFixVector3Drawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty xProp = property.FindPropertyRelative("X").FindPropertyRelative("m_rawValue");
        SerializedProperty yProp = property.FindPropertyRelative("Y").FindPropertyRelative("m_rawValue");
        SerializedProperty zProp = property.FindPropertyRelative("Z").FindPropertyRelative("m_rawValue");

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

            xProp.longValue = newFixVec.X.m_rawValue;
            yProp.longValue = newFixVec.Y.m_rawValue;
            zProp.longValue = newFixVec.Z.m_rawValue;
        }


        EditorGUI.EndProperty();
    }
}
