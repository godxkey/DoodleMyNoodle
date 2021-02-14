using CCC.Editor;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using Unity.Properties.Adapters;
using UnityEditor;
using UnityEngine;

[CustomEntityPropertyDrawer]
public class Fix2DPropertyDrawer : IMGUIAdapter,
    IVisit<fix>,
    IVisit<fix2>,
    IVisit<fix3>
{

    public VisitStatus Visit<TContainer>(Property<TContainer, fix> property, ref TContainer container, ref fix value)
    {
        EditorGUILayout.FloatField(GetDisplayName(property), (float)value);
        return VisitStatus.Stop;
    }

    public VisitStatus Visit<TContainer>(Property<TContainer, fix2> property, ref TContainer container, ref fix2 value)
    {
        EditorGUILayout.Vector2Field(GetDisplayName(property), new Vector2((float)value.x, (float)value.y));
        return VisitStatus.Stop;
    }

    public VisitStatus Visit<TContainer>(Property<TContainer, fix3> property, ref TContainer container, ref fix3 value)
    {
        EditorGUILayout.Vector2Field(GetDisplayName(property), new Vector3((float)value.x, (float)value.y, (float)value.z));
        return VisitStatus.Stop;
    }
}