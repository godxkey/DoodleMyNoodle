using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;

public class GameActionShield : GameAction
{
    public override Type[] GetRequiredSettingTypes() => new Type[]
    {
        typeof(GameActionSettingRange),
        typeof(GameActionSettingEffectDuration),
    };

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        if (accessor.GetComponentData<GameActionSettingRange>(context.Item).Value > 0)
        {
            return new UseContract(
                new GameActionParameterEntity.Description()
                {
                    RangeFromInstigator = accessor.GetComponentData<GameActionSettingRange>(context.Item),
                    IncludeSelf = true,
                    RequiresAttackableEntity = true,
                });
        }
        else
        {
            return new UseContract();
        }
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData, ref ResultData resultData)
    {
        if (useData.TryGetParameter(0, out GameActionParameterEntity.Data paramEntity))
        {
            Entity target = CommonReads.Physics.FindFirstEntityWithComponentAtPosition<Health>(accessor, paramEntity.EntityPos);
            ShieldTarget(accessor, context.Item, target);
            return true;
        }

        LogGameActionInfo(context, "Wrong parameters");

        return false;
    }

    private void ShieldTarget(ISimWorldReadWriteAccessor accessor, Entity itemEntity, Entity pawn)
    {
        int duration = accessor.GetComponentData<GameActionSettingEffectDuration>(itemEntity).Value;

        if (accessor.TryGetComponentData(pawn, out Invincible invincible))
        {
            accessor.AddComponentData(pawn, new Invincible() { Duration = max(duration, invincible.Duration) });
        }
        else
        {
            accessor.AddComponentData(pawn, new Invincible() { Duration = duration });
        }
    }
}

