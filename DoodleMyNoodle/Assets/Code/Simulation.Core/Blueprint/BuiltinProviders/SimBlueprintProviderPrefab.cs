using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    public void ProvideBlueprintBatched(in SimBlueprintId[] blueprintIds, Action<SimBlueprint[]> onComplete)
    {
        onComplete(blueprintIds.Select((id) => ProvideBlueprint(id)).ToArray());
    }

    public void ReleaseBatchedBlueprints()
    {
    }

    public static SimBlueprintId MakeBlueprintId(string prefabGuid)
    {
        return new SimBlueprintId(SimBlueprintId.BlueprintType.Prefab, prefabGuid);
    }

}
