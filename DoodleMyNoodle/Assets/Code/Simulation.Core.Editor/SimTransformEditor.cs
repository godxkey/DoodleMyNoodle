using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimTransform), editorForChildClasses: true)]
public class SimTransformEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (Application.isPlaying)
        {
            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            if (EditorGUI.EndChangeCheck())
            {
                ((SimTransform)target).Editor_DirtyCachedAllValues();
            }
        }
        else
        {
            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            SimTransform simTransform = (SimTransform)target;
            Transform unityTransform = simTransform.gameObject.transform;

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(unityTransform, "Change UnityTransform");

                unityTransform.localPosition = simTransform.localPosition.ToUnityVec();
                unityTransform.localRotation = simTransform.localRotation.ToUnityQuat();
                unityTransform.localScale = simTransform.localScale.ToUnityVec();

                PrefabUtility.RecordPrefabInstancePropertyModifications(target);
            }
            else
            {
                FixVector3 localPosition = unityTransform.localPosition.ToFixVec();
                FixQuaternion localRotation = unityTransform.localRotation.ToFixQuat();
                FixVector3 localScale = unityTransform.localScale.ToFixVec();

                bool change = localPosition != simTransform.localPosition
                            || localRotation != simTransform.localRotation
                            || localScale != simTransform.localScale;

                if (change)
                {
                    Undo.RecordObject(target, "Change SimTransform");

                    simTransform.localPosition = localPosition;
                    simTransform.localRotation = localRotation;
                    simTransform.localScale = localScale;

                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                }
            }
        }
    }
}
