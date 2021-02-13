using SimulationControl;
using Unity.Entities;
using UnityEngine;

public static class PresentationHelpers
{
    public static World PresentationWorld => World.DefaultGameObjectInjectionWorld;
    public static ExternalSimWorldAccessor GetSimulationWorld() => GetPresentationWorldSystem<SimulationWorldSystem>()?.SimWorldAccessor;

    public static void SubmitInput(SimInput input, bool throwErrorIfFailed = false)
    {
        var submitSystem = PresentationWorld?.GetExistingSystem<SubmitSimulationInputSystem>();
        if (submitSystem != null)
        {
            submitSystem.SubmitInput(input);
        }
        else
        {
            if (throwErrorIfFailed)
                throw new System.Exception($"Failed to submit input: {input}. Could not find '{nameof(SubmitSimulationInputSystem)}'");
        }
    }

    public static T GetPresentationWorldSystem<T>() where T : ComponentSystem
    {
        return PresentationWorld?.GetExistingSystem<T>();
    }

    public static GameObject FindSimAssetPrefab(SimAssetId simAssetId)
    {
        return SimAssetBankInstance.GetLookup().GetSimAsset(simAssetId)?.gameObject;
    }

    public static GameObject FindBindedView(Entity simEntity)
    {
        if (BindedSimEntityManaged.InstancesMap.TryGetValue(simEntity, out GameObject result))
        {
            return result;
        }
        return null;
    }
}