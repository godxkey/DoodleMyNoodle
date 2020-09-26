using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ItemExplosionRangeAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescription<ItemHealthPointCostData>
{
    public int ExplosionRange;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ItemExplosionRange() { Value = ExplosionRange });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription(ItemHealthPointCostData inputData)
    {
        if (inputData.Value == ExplosionRange)
        {
            return $"Explosion Range : {inputData.Value}";
        }
        else
        {
            return $"Explosion Range : {inputData.Value} ({ExplosionRange})";
        }
    }
}