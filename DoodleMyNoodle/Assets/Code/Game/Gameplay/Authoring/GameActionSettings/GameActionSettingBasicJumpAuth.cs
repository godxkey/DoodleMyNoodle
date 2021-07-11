using UnityEngine;
using System;
using Unity.Entities;

[Serializable]
[GameActionSettingAuthAttribute(typeof(GameActionSettingBasicJump))]
public class GameActionSettingBasicJumpAuth : GameActionSettingAuthBase
{
    public fix JumpVelocity;
    public fix EnergyCost;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionSettingBasicJump()
        {
            JumpVelocity = JumpVelocity,
            EnergyCost = EnergyCost,
        });
    }
}