using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class SimAssetIdAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public string Guid;

    private bool _hasCreatedRuntimeId = false;
    private SimAssetId _runtimeId;

    public SimAssetId GetSimAssetId()
    {
        if (_hasCreatedRuntimeId)
        {
            return _runtimeId;
        }
        else
        {
            _hasCreatedRuntimeId = true;

            if (SimAssetIdMapInstance.Get() != null)
            {
                _runtimeId = new SimAssetId()
                {
                    Value = SimAssetIdMapInstance.Get().EditIdToRuntimeId(Guid)
                };
            }
            else
            {
                Debug.LogError("SimAssetIdMapInstance is null. The converted SimAssetId will be invalid.");
            }

            return _runtimeId;
        }
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, GetSimAssetId());
    }
}
