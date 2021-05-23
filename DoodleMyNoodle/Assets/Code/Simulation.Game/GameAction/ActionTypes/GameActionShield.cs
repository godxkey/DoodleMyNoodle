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
        if (accessor.GetComponent<GameActionSettingRange>(context.Item).Value > 0)
        {
            return new UseContract(
                new GameActionParameterEntity.Description()
                {
                    RangeFromInstigator = accessor.GetComponent<GameActionSettingRange>(context.Item),
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
            var settingsRange = accessor.GetComponent<GameActionSettingRange>(context.Item);

            if (!accessor.Exists(paramEntity.Entity))
            {
                LogGameActionInfo(context, "Target does not exist");
                return false;
            }

            if (!CommonReads.IsInRange(accessor, context.InstigatorPawn, paramEntity.Entity, settingsRange))
            {
                LogGameActionInfo(context, "Target out of range");
                return false;
            }

            ShieldTarget(accessor, context.Item, paramEntity.Entity);
            return true;
        }

        LogGameActionInfo(context, "Wrong parameters");

        return false;
    }

    private void ShieldTarget(ISimWorldReadWriteAccessor accessor, Entity itemEntity, Entity pawn)
    {
        int duration = accessor.GetComponent<GameActionSettingEffectDuration>(itemEntity).Value;

        if (accessor.TryGetComponent(pawn, out Invincible invincible))
        {
            accessor.AddComponent(pawn, new Invincible() { Duration = max(duration, invincible.Duration) });
        }
        else
        {
            accessor.AddComponent(pawn, new Invincible() { Duration = duration });
        }
    }
}

