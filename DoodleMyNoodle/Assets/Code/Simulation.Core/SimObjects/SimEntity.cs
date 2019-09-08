using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[DisallowMultipleComponent]
public class SimEntity : SimObject
{
    public SimEntityId EntityId { get; internal set; }
    public SimBlueprintId BlueprintId { get; internal set; }

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
            SimModules.EntityManager.OnDestroyEntity(this);
        }
        IsPartOfSimulationRuntime = false;
    }
}
