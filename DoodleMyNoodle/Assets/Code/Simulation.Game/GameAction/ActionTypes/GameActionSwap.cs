using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Entities;
using Unity.Collections;
using CCC.Fix2D;
using System;
using System.Collections.Generic;

public class GameActionSwap : GameAction<GameActionSwap.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public fix Range = 1;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings() { Range = Range });
        }
    }

    public struct Settings : IComponentData
    {
        public fix Range;
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context, Settings settings)
    {        
        return new UseContract(
            new GameActionParameterEntity.Description()
            {
                RangeFromInstigator = settings.Range,
                IncludeSelf = false,
            });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData, List<ResultDataElement> resultData, Settings settings)
    {
        if (useData.TryGetParameter(0, out GameActionParameterEntity.Data paramEntity))
        {
            if (!accessor.Exists(paramEntity.Entity))
            {
                LogGameActionInfo(context, "Target does not exit");
            }

            if (!CommonReads.IsInRange(accessor, context.InstigatorPawn, paramEntity.Entity, settings.Range))
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
