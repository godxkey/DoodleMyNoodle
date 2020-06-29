using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemHealthPointsToHealDataAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public int HealthToHeal;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ItemHealthPointsToHealData() { Value = HealthToHeal });
    }
}