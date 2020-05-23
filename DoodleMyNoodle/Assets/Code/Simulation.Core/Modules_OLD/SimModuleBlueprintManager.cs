using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngineX;

internal class SimModuleBlueprintManager : SimModuleBase
{
    ISimBlueprintProvider[] _blueprintProviders;
    Coroutine _provideBlueprintBatchedRoutine;

    internal override void Initialize(SimulationCoreSettings settings)
    {
        if (settings.BlueprintProviders != null)
        {
            _blueprintProviders = new ISimBlueprintProvider[settings.BlueprintProviders.Count];
            settings.BlueprintProviders.CopyTo(_blueprintProviders);
        }
    }

    public SimBlueprint GetBlueprint(in SimBlueprintId blueprintId)
    {
        ISimBlueprintProvider bpProvider = GetBlueprintProviderForBlueprintId(blueprintId);
        if (bpProvider == null)
        {
            return default;
        }

        return bpProvider.ProvideBlueprint(blueprintId);
    }

    public IEnumerator ProvideBlueprintBatched(List<SimBlueprintId> requestedBlueprintIds, Action<SimBlueprint[]> onComplete)
    {
        // Ask every provider to provide blueprints from the blueprintIds
        SimBlueprint[][] providerResults = new SimBlueprint[_blueprintProviders.Length][];
        for (int i = 0; i < _blueprintProviders.Length; i++)
        {
            SimBlueprintId[] request = requestedBlueprintIds.Where((x) => _blueprintProviders[i].CanProvideBlueprintFor(x)).ToArray();

            int index = i; // needed to create a copy of 'i' for the callback
            _blueprintProviders[i].ProvideBlueprintBatched(request, (blueprints) => providerResults[index] = blueprints);
        }

        // wait until all providers have responded with their results
        while (providerResults.ContainsNull())
        {
            yield return null;
        }

        // LOCAL FUNCTION
        SimBlueprint FindBlueprintInProviderResults(in SimBlueprintId blueprintId)
        {
            for (int i = 0; i < providerResults.Length; i++)
            {
                for (int j = 0; j < providerResults[i].Length; j++)
                {
                    if (providerResults[i][j].Id == blueprintId)
                    {
                        return providerResults[i][j];
                    }
                }
            }
            return new SimBlueprint();
        }

        // group blueprints back together, with the same sorting as the requested 'requestedBlueprintIds'
        SimBlueprint[] combinedResult = new SimBlueprint[requestedBlueprintIds.Count];
        for (int i = 0; i < requestedBlueprintIds.Count; i++)
        {
            combinedResult[i] = FindBlueprintInProviderResults(requestedBlueprintIds[i]);
        }

        // callback
        onComplete(combinedResult);
    }

    public void ReleaseBatchedBlueprints()
    {
        _provideBlueprintBatchedRoutine = null;
        foreach (var provider in _blueprintProviders)
        {
            provider.ReleaseBatchedBlueprints();
        }
    }

    ISimBlueprintProvider GetBlueprintProviderForBlueprintId(in SimBlueprintId blueprintId)
    {
        if (_blueprintProviders != null)
        {
            for (int i = 0; i < _blueprintProviders.Length; i++)
            {
                if (_blueprintProviders[i].CanProvideBlueprintFor(blueprintId))
                    return _blueprintProviders[i];
            }
        }

        DebugService.LogError($"Could not find blueprint provider for blueprint id {blueprintId}");

        return null;
    }

    public override void Dispose()
    {
        if (_provideBlueprintBatchedRoutine != null)
        {
            CoroutineLauncherService.Instance.StopCoroutine(_provideBlueprintBatchedRoutine);
            _provideBlueprintBatchedRoutine = null;
        }
        base.Dispose();
    }
}
