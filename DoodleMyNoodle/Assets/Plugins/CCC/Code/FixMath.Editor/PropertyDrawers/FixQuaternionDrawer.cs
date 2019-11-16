using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(FixQuaternion))]
public class FixQuaternionDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedProperty xProp = property.FindPropertyRelative(nameof(Quaternion.x)).FindPropertyRelative(nameof(Fix64.RawValue));
        SerializedProperty yProp = property.FindPropertyRelative(nameof(Quaternion.y)).FindPropertyRelative(nameof(Fix64.RawValue));
        SerializedProperty zProp = property.FindPropertyRelative(nameof(Quaternion.z)).FindPropertyRelative(nameof(Fix64.RawValue));
        SerializedProperty wProp = property.FindPropertyRelative(nameof(Quaternion.w)).FindPropertyRelative(nameof(Fix64.RawValue));

        Fix64 xVal;
        Fix64 yVal;
        Fix64 zVal;
        Fix64 wVal;
        xVal.RawValue = xProp.longValue;
        yVal.RawValue = yProp.longValue;
        zVal.RawValue = zProp.longValue;
        wVal.RawValue = wProp.longValue;

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

            xProp.longValue = newQuat.x.RawValue;
            yProp.longValue = newQuat.y.RawValue;
            zProp.longValue = newQuat.z.RawValue;
            wProp.longValue = newQuat.w.RawValue;
        }


        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(SerializedPropertyType.Vector3, label);
    }
}
