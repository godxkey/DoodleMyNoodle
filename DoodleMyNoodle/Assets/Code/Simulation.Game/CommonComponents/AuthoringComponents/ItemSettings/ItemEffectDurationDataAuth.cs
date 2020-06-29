using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemEffectDurationDataAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public int Duration;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ItemEffectDurationData() { Value = Duration });
    }
}