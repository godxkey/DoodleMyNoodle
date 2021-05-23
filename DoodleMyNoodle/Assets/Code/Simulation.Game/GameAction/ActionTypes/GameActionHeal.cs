using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;
using Unity.Entities;
using Unity.Collections;

public class GameActionHeal : GameAction
{
    public override Type[] GetRequiredSettingTypes() => new Type[]
    {
        typeof(GameActionSettingRange),
        typeof(GameActionSettingHPToHeal)
    };

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        UseContract useContract = new UseContract();
        useContract.ParameterTypes = new ParameterDescription[]
        {
            new GameActionParameterEntity.Description()
            {
                RangeFromInstigator = accessor.GetComponent<GameActionSettingRange>(context.Item).Value,
                RequiresAttackableEntity = true,
            }
        };

        return useContract;
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterEntity.Data paramEntity))
        {
            var settingsHeal = accessor.GetComponent<GameActionSettingHPToHeal>(context.Item);
            var settingsRange = accessor.GetComponent<GameActionSettingRange>(context.Item);

            if (!accessor.Exists(paramEntity.Entity))
            {
                LogGameActionInfo(context, "Target does not exist");
                return false;
            }

            if(!CommonReads.IsInRange(accessor, context.InstigatorPawn, paramEntity.Entity, settingsRange))
            {
                LogGameActionInfo(context, "Target out of range");
                return false;
            }

            if (!accessor.HasComponent<Health>(paramEntity.Entity))
            {
                LogGameActionInfo(context, "Target has no health");
                return false;
            }

            CommonWrites.RequestHeal(accessor, context.InstigatorPawn, paramEntity.Entity, settingsHeal.Value);

            return true;
        }

        return false;
    }
}
