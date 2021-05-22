using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Entities;
using Unity.Collections;
using CCC.Fix2D;
using System;

public class GameActionSwap : GameAction
{
    public override Type[] GetRequiredSettingTypes() => new Type[]
    {
        typeof(GameActionSettingRange),
    };

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return new UseContract(
            new GameActionParameterEntity.Description()
            {
                RangeFromInstigator = accessor.GetComponent<GameActionSettingRange>(context.Item),
                IncludeSelf = false,
            });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData, ref ResultData resultData)
    {
        if (useData.TryGetParameter(0, out GameActionParameterEntity.Data paramEntity))
        {
            var settingsRange = accessor.GetComponent<GameActionSettingRange>(context.Item);

            if (!accessor.Exists(paramEntity.Entity))
            {
                LogGameActionInfo(context, "Target does not exit");
            }

            if (!CommonReads.IsInRange(accessor, context.InstigatorPawn, paramEntity.Entity, settingsRange))
            {
                LogGameActionInfo(context, "Target is out of range");
            }

            fix2 targetPos = accessor.GetComponent<FixTranslation>(paramEntity.Entity);
            fix2 instigatorPos = accessor.GetComponent<FixTranslation>(context.InstigatorPawn);
            
            // swap positions
            CommonWrites.RequestTeleport(accessor, context.InstigatorPawn, targetPos);
            CommonWrites.RequestTeleport(accessor, paramEntity.Entity, instigatorPos);

            return true;
        }

        return false;
    }
}
