using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SimTransform))]
public class SimTransformEditor : Editor
{
    public override void OnInspectorGUI()
    {
        if (Application.isPlaying)
        {
            base.OnInspectorGUI();
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
                FixVector3 localPosition = unityTransform.localPosition.ToFixVecCopy();
                FixQuaternion localRotation = unityTransform.localRotation.ToFixQuatCopy();
                FixVector3 localScale = unityTransform.localScale.ToFixVecCopy();

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
