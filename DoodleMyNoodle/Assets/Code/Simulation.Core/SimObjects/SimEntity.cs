using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

[DisallowMultipleComponent]
public class SimEntity : SimObject
{
    public SimEntityId entityId { get; internal set; }
    public SimBlueprintId blueprintId { get; internal set; }

    [field: System.NonSerialized]
    public bool isPartOfSimulation { get; private set; }

    public override void OnAddedToEntityList()
    {
        isPartOfSimulation = true;
    }
    public override void OnRemovingFromEntityList()
    {
        isPartOfSimulation = false;
    }

    private void OnDestroy()
    {
        if (isPartOfSimulation && !ApplicationUtilityService.ApplicationIsQuitting && SimModules.isInitialized)
        {
            SimModules.entityManager.OnDestroyEntity(this);
        }
    }
}
