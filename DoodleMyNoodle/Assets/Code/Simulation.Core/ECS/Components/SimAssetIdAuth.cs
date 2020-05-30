using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class SimAssetIdAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public string Guid;

    public SimAssetId GetSimAssetId()
    {
        if (SimAssetIdMapInstance.Get() != null)
        {
            return new SimAssetId()
            {
                Value = SimAssetIdMapInstance.Get().EditIdToRuntimeId(Guid)
            };
        }
        else
        {
            Debug.LogError("SimAssetIdMapInstance is null. The converted SimAssetId will be invalid.");
        }

        return SimAssetId.Invalid;
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, GetSimAssetId());
    }
}
