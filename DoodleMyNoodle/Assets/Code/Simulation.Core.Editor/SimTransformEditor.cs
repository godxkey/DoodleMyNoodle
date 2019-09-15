using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimTransformComponent), editorForChildClasses: true)]
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
                ((SimTransformComponent)target).Editor_DirtyCachedAllValues();
            }
        }
        else
        {
            EditorGUI.BeginChangeCheck();

            base.OnInspectorGUI();

            SimTransformComponent simTransform = (SimTransformComponent)target;
            Transform unityTransform = simTransform.gameObject.transform;

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(unityTransform, "Change UnityTransform");

                unityTransform.localPosition = simTransform.LocalPosition.ToUnityVec();
                unityTransform.localRotation = simTransform.LocalRotation.ToUnityQuat();
                unityTransform.localScale = simTransform.LocalScale.ToUnityVec();

                PrefabUtility.RecordPrefabInstancePropertyModifications(target);
            }
            else
            {
                FixVector3 localPosition = unityTransform.localPosition.ToFixVec();
                FixQuaternion localRotation = unityTransform.localRotation.ToFixQuat();
                FixVector3 localScale = unityTransform.localScale.ToFixVec();

                bool change = localPosition != simTransform.LocalPosition
                            || localRotation != simTransform.LocalRotation
                            || localScale != simTransform.LocalScale;

                if (change)
                {
                    Undo.RecordObject(target, "Change SimTransform");

                    simTransform.LocalPosition = localPosition;
                    simTransform.LocalRotation = localRotation;
                    simTransform.LocalScale = localScale;

                    PrefabUtility.RecordPrefabInstancePropertyModifications(target);
                }
            }
        }
    }
}
