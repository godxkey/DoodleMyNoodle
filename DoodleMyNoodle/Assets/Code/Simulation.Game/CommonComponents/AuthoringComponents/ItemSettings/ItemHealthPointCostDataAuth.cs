using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemHealthPointCostDataAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public int HealthCost;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ItemHealthPointCostData() { Value = HealthCost });
    }
}