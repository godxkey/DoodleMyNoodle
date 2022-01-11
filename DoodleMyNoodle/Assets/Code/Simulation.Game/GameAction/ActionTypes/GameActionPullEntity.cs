using static fixMath;
using Unity.Entities;
using Unity.Collections;
using System;
using System.Collections.Generic;
using CCC.Fix2D;

public class GameActionPullEntity : GameAction<GameActionPullEntity.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public fix Speed;
        public fix Range;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                Speed = Speed,
                Range = Range,
            });
        }
    }

    public struct Settings : IComponentData
    {
        public fix Speed;
        public fix Range;
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context, Settings settings)
    {
        return new UseContract(
                   new GameActionParameterEntity.Description()
                   {
                       RangeFromInstigator = settings.Range,
                       IncludeSelf = false,
                       RequiresAttackableEntity = true
                   });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, List<ResultDataElement> resultData, Settings settings)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterEntity.Data paramEntity))
        {
            if (accessor.TryGetComponent(context.InstigatorPawn, out FixTranslation translation))
            {
                CommonWrites.RequestPull(accessor, paramEntity.Entity, translation.Value, settings.Speed);
            }

            return true;
        }

        return false;
    }
}