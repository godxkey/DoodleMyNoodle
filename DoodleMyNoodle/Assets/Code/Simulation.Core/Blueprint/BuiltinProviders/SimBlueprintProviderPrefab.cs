using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimBlueprintProviderPrefab : MonoBehaviour, ISimBlueprintProvider
{
    public SimPrefabBank SimPrefabBank;

    public bool CanProvideBlueprintFor(in SimBlueprintId blueprintId) => blueprintId.Type == SimBlueprintId.BlueprintType.Prefab;

    public bool CanProvideBlueprintSynchronously() => true;

    public SimBlueprint ProvideBlueprint(in SimBlueprintId blueprintId)
    {
        ReadOnlyList<SimPrefabBank.PrefabData> prefabData = SimPrefabBank.PrefabBlueprints;

        for (int i = 0; i < prefabData.Count; i++)
        {
            if (prefabData[i].BlueprintId.Value == blueprintId.Value)
                return new SimBlueprint(blueprintId, prefabData[i].Prefab);
        }
        return default;
    }

    public void ProvideBlueprintAsync(in SimBlueprintId blueprintId, Action<SimBlueprint> onComplete)
    {
        onComplete(ProvideBlueprint(blueprintId));
    }

    public static SimBlueprintId MakeBlueprintId(string prefabGuid)
    {
        return new SimBlueprintId(SimBlueprintId.BlueprintType.Prefab, prefabGuid);
    }

    public void ProvideBlueprintsAsync(in List<SimBlueprintId> blueprintIds, Action<List<SimBlueprint>> onComplete)
    {
        throw new NotImplementedException();
    }

    public void EndProvideBlueprint()
    {
        throw new NotImplementedException();
    }
}
