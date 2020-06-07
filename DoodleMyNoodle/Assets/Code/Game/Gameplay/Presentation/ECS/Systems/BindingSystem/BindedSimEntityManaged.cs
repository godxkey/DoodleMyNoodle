using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BindedSimEntityManaged : MonoBehaviour, IIndexedInList, ISystemStateComponentData
{
    public Entity SimEntity;

    // cached components
    public Transform Transform;

    private void Awake()
    {
        s_instances.AddIndexed(this);

        Transform = transform;
    }

    private void OnDestroy()
    {
        s_instances.RemoveIndexed(this);
    }

    public static ReadOnlyList<BindedSimEntityManaged> Instances => s_instances.AsReadOnlyNoAlloc();
    private static List<BindedSimEntityManaged> s_instances = new List<BindedSimEntityManaged>();
    int IIndexedInList.Index { get; set; }
}

#if UNITY_EDITOR
[CustomEditor(typeof(BindedSimEntityManaged))]
public class BindedSimEntityManagedEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GameMonoBehaviourHelpers.GetSimulationWorld() != null)
        {
            Entity simEntity = ((BindedSimEntityManaged)target).SimEntity;
            if (GameMonoBehaviourHelpers.GetSimulationWorld().Exists(simEntity))
            {
                string entityName = GameMonoBehaviourHelpers.GetSimulationWorld().GetName(simEntity);
                EditorGUILayout.LabelField($"Binded to: {entityName} - {simEntity}", EditorStyles.boldLabel);
            }
            else
            {
                EditorGUILayout.LabelField($"Binded to destroyed entity: {simEntity}", EditorStyles.boldLabel);
            }


            //TODO: Test: destroying the sim entity destroys the gameobject
            //      Implenent: transform is copied to gameobject
        }
    }
}
#endif