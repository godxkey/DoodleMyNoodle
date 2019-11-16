using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

internal class SimModuleBlueprintManager : SimModuleBase
{
    ISimBlueprintProvider[] _blueprintProviders;

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

    public void GetBlueprintAsync(in SimBlueprintId blueprintId, Action<SimBlueprint> onComplete)
    {
        ISimBlueprintProvider bpProvider = GetBlueprintProviderForBlueprintId(blueprintId);
        if (bpProvider == null)
        {
            onComplete(default);
        }
        else
        {
            bpProvider.ProvideBlueprintAsync(blueprintId, onComplete);
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
}
