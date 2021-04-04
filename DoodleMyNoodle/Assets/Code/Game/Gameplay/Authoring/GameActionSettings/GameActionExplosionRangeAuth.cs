using System;
using Unity.Entities;
using UnityEngine;

[RequiresEntityConversion]
[Serializable]
[GameActionSettingAuth(typeof(GameActionHPCostData))]
public class GameActionExplosionRangeAuth : GameActionSettingAuthBase, IConvertGameObjectToEntity, IItemSettingDescription
{
    public int ExplosionRange;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionExplosionRange() { Value = ExplosionRange });
    }

    public Color GetColor()
    {
        return Color.white;
    }

    public string GetDescription()
    {
        return $"Explosion Range : {ExplosionRange}";
    }
}