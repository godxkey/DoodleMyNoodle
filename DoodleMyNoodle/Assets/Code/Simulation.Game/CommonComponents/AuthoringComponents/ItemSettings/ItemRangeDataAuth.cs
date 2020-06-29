using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemRangeDataAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public int Range;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ItemRangeData() { Value = Range });
    }
}