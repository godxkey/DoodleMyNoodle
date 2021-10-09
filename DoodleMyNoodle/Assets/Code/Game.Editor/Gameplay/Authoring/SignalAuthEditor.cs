using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(SignalAuth))]
public class SignalAuthEditor : Editor
{
    private SerializedProperty StayOnForever;
    private SerializedProperty Emission;
    private SerializedProperty LogicTargets;
    //private SerializedProperty PropagationTargets;

    private void OnEnable()
    {
        StayOnForever = serializedObject.FindProperty(nameof(SignalAuth.StayOnForever));
        Emission = serializedObject.FindProperty(nameof(SignalAuth.Emission));
        LogicTargets = serializedObject.FindProperty(nameof(SignalAuth.LogicTargets));
        //PropagationTargets = serializedObject.FindProperty(nameof(SignalAuth.PropagationTargets));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(StayOnForever);

        EditorGUILayout.PropertyField(Emission);

        var type = (ESignalEmissionType)Emission.intValue;
        if (type == ESignalEmissionType.AND || type == ESignalEmissionType.OR)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(LogicTargets);
            EditorGUI.indentLevel--;
        }

        //EditorGUILayout.Space();
        //EditorGUILayout.PropertyField(PropagationTargets);

        serializedObject.ApplyModifiedProperties();
    }
}