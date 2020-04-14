using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class SimAssetIdAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public int Value;
    
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new SimAssetId() { Value = Value });
    }
}
