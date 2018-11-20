using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UnityObjectVariable))]
public class UnityObjectVariableEditor : VarVariableEditor<Object>
{
    protected override Object DrawRuntimeValueField()
    {
        return EditorGUILayout.ObjectField("Label:", variable.Value, typeof(Object), true);
    }
}
