using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[CustomEditor(typeof(MovingPlatformAuth))]
public class MovingPlatformAuthEditor : Editor
{
    private SerializedProperty MovePoints;
    private SerializedProperty MaximumSpeed;
    private SerializedProperty Move;
    private SerializedProperty SlowDownNearPoints;
    private SerializedProperty PauseOnPoints;
    private SerializedProperty PauseDuration;

    private void OnEnable()
    {
        MovePoints = serializedObject.FindProperty(nameof(MovingPlatformAuth.MovePoints));
        MaximumSpeed = serializedObject.FindProperty(nameof(MovingPlatformAuth.MaximumSpeed));
        Move = serializedObject.FindProperty(nameof(MovingPlatformAuth.MoveMode));
        SlowDownNearPoints = serializedObject.FindProperty(nameof(MovingPlatformAuth.SlowDownNearPoints));
        PauseOnPoints = serializedObject.FindProperty(nameof(MovingPlatformAuth.PauseOnPoints));
        PauseDuration = serializedObject.FindProperty(nameof(MovingPlatformAuth.PauseDuration));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(Move);

        var moveMode = (PlatformMoveMode)Move.intValue;

        MovingPlatformAuthPointDrawer.DisplaySignal = moveMode == PlatformMoveMode.Signals;
        EditorGUILayout.PropertyField(MovePoints);

        if (moveMode == PlatformMoveMode.Signals)
        {
            bool pointsWillBeIgnored = false;
            // If any of the points don't have an assigned signal emitter, all following points will be ignored. Display a warning
            for (int i = 0; i < MovePoints.arraySize - 1; i++)
            {
                var conditionalEmitterProp = MovePoints.GetArrayElementAtIndex(i).FindPropertyRelative(nameof(MovingPlatformAuth.Point.ConditionalEmitter));
                if (conditionalEmitterProp.objectReferenceValue == null)
                {
                    pointsWillBeIgnored = true;
                    break;
                }
            }

            if (pointsWillBeIgnored)
            {
                EditorGUILayout.HelpBox($"When no {nameof(MovingPlatformAuth.Point.ConditionalEmitter)} is assigned, all following points will be ignored.", MessageType.Warning);
            }
        }

        EditorGUILayout.PropertyField(MaximumSpeed);
        EditorGUILayout.PropertyField(SlowDownNearPoints);

        EditorGUILayout.PropertyField(PauseOnPoints);
        if (PauseOnPoints.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(PauseDuration);
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}

[CustomPropertyDrawer(typeof(MovingPlatformAuth.Point))]
public class MovingPlatformAuthPointDrawer : PropertyDrawer
{
    public static bool DisplaySignal = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        position.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(MovingPlatformAuth.Point.Position)), new GUIContent("Position"));

        if (DisplaySignal)
        {
            position.position += Vector2.up * (position.height + EditorGUIUtility.standardVerticalSpacing);
            EditorGUI.PropertyField(position, property.FindPropertyRelative(nameof(MovingPlatformAuth.Point.ConditionalEmitter)), new GUIContent("Conditional Signal"));
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (DisplaySignal)
        {
            return EditorGUIUtility.singleLineHeight * 2
                + EditorGUIUtility.standardVerticalSpacing;
        }
        else
        {
            return EditorGUIUtility.singleLineHeight;
        }
    }

}