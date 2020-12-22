using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class GameActionExplosionRangeAuth : MonoBehaviour, IConvertGameObjectToEntity, IItemSettingDescription<GameActionHPCostData>
{
    public int ExplosionRange;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionExplosionRange() { Value = ExplosionRange });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription(GameActionHPCostData inputData)
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