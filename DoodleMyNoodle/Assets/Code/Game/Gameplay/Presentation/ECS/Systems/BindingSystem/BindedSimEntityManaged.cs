using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;
using System.Collections.ObjectModel;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class BindedSimEntityManaged : MonoBehaviour, IIndexedInList, ISystemStateComponentData
{
    public Entity SimEntity { get; private set; }

    // cached components
    public Transform Transform;

    public void Init(Entity simEntity)
    {
        SimEntity = simEntity;

        s_instancesMap.Add(simEntity, gameObject);
    }

    private void Awake()
    {
        s_instances.AddIndexed(this);

        Transform = transform;
    }

    private void OnDestroy()
    {
        s_instances.RemoveIndexed(this);
        s_instancesMap.Remove(SimEntity);
    }

    int IIndexedInList.Index { get; set; }

    
    public static ReadOnlyDictionary<Entity, GameObject> InstancesMap
    {
        get
        {
            if(s_instancesMapRO == null)
            {
                s_instancesMapRO = new ReadOnlyDictionary<Entity, GameObject>(s_instancesMap);
            }

            return s_instancesMapRO;
        }
    }

    public static ReadOnlyList<BindedSimEntityManaged> Instances => s_instances.AsReadOnlyNoAlloc();

    private static Dictionary<Entity, GameObject> s_instancesMap = new Dictionary<Entity, GameObject>();
    private static ReadOnlyDictionary<Entity, GameObject> s_instancesMapRO = new ReadOnlyDictionary<Entity, GameObject>(s_instancesMap);
    private static List<BindedSimEntityManaged> s_instances = new List<BindedSimEntityManaged>();
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