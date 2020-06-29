using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemActionPointCostDataAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public int ActionPointCost;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ItemActionPointCostData() { Value = ActionPointCost });
    }
}