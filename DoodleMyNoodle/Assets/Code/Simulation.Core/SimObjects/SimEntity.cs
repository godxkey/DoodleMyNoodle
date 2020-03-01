using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using CCC.InspectorDisplay;

#if UNITY_EDITOR
using UnityEditor;
#endif

[DisallowMultipleComponent]
public class SimEntity : SimObject
{
    [SerializeField, ReadOnlyAlways]
    private SimBlueprintId _blueprintId;
    public SimBlueprintId BlueprintId { get => _blueprintId; internal set => _blueprintId = value; }

    [field: System.NonSerialized]
    public bool IsPartOfSimulationRuntime { get; private set; }

    public override void OnAddedToRuntime()
    {
        IsPartOfSimulationRuntime = true;
    }
    public override void OnRemovingFromRuntime()
    {
        IsPartOfSimulationRuntime = false;
    }

    private void OnDestroy()
    {
        if (IsPartOfSimulationRuntime && !ApplicationUtilityService.ApplicationIsQuitting && SimModules.IsInitialized)
        {
            SimModules._EntityManager.OnDestroyEntity(this);
        }
        IsPartOfSimulationRuntime = false;
    }


#if UNITY_EDITOR
    [MenuItem("CONTEXT/SimEntity/Print SimBlueprintId")]
    static void PrintSimBlueprintId(MenuCommand command)
    {
        SimEntity obj = (SimEntity)command.context;
        Debug.Log(obj.BlueprintId);
    }
#endif
}
