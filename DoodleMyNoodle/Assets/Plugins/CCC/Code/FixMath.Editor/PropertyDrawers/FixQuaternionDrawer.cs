using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FixQuaternion))]
public class FixQuaternionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty xProp = property.FindPropertyRelative("X").FindPropertyRelative("m_rawValue");
        SerializedProperty yProp = property.FindPropertyRelative("Y").FindPropertyRelative("m_rawValue");
        SerializedProperty zProp = property.FindPropertyRelative("Z").FindPropertyRelative("m_rawValue");
        SerializedProperty wProp = property.FindPropertyRelative("W").FindPropertyRelative("m_rawValue");

        Fix64 xVal;
        Fix64 yVal;
        Fix64 zVal;
        Fix64 wVal;
        xVal.m_rawValue = xProp.longValue;
        yVal.m_rawValue = yProp.longValue;
        zVal.m_rawValue = zProp.longValue;
        wVal.m_rawValue = wProp.longValue;

        FixQuaternion oldQuat = new FixQuaternion(xVal, yVal, zVal, wVal);        

        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        // Editor Field
        Vector3 oldEuler = oldQuat.ToUnityQuat().eulerAngles;
        Vector3 newEuler =  EditorGUI.Vector3Field(position, label, oldEuler);

        // Change ?
        if (oldEuler != newEuler)
        {
            FixQuaternion newQuat = Quaternion.Euler(newEuler.x, newEuler.y, newEuler.z).ToFixQuat();

            xProp.longValue = newQuat.X.m_rawValue;
            yProp.longValue = newQuat.Y.m_rawValue;
            zProp.longValue = newQuat.Z.m_rawValue;
            wProp.longValue = newQuat.W.m_rawValue;
        }


        EditorGUI.EndProperty();
    }
}
