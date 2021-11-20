using static fixMath;
using Unity.Entities;
using Unity.Collections;
using System;
using CCC.Fix2D;

public class GameActionThrowNearbyEntity : GameAction<GameActionThrowNearbyEntity.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public fix Range;
        public fix SpeedMin = 0;
        public fix SpeedMax = 10;
        public fix SpawnExtraDistance = 0;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                Range = Range,
                SpeedMax = SpeedMax,
                SpeedMin = SpeedMin,
                SpawnExtraDistance = SpawnExtraDistance,
            });
        }
    }

    public struct Settings : IComponentData
    {
        public fix Range;
        public fix SpeedMax;
        public fix SpeedMin;
        public fix SpawnExtraDistance;
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context, Settings settings)
    {
        return new UseContract(
                   new GameActionParameterEntity.Description()
                   {
                       RangeFromInstigator = settings.Range,
                       IncludeSelf = false,
                       CustomPredicate = (simWorld, tileActor) =>
                       {
                           if (simWorld.HasComponent<FixTranslation>(tileActor) && simWorld.HasComponent<Health>(tileActor))
                           {
                               return true;
                           }

                           return false;
                       }
                   },
                   new GameActionParameterVector.Description()
                   {
                       SpeedMin = settings.SpeedMin,
                       SpeedMax = settings.SpeedMax,
                       UsePreviousParameterOriginLocation = true
                   });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData, Settings settings)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterEntity.Data paramEntity))
        {
            Entity entityToThrow = paramEntity.Entity;

            if (parameters.TryGetParameter(1, out GameActionParameterVector.Data paramVector))
            {
                fix2 velocity = clampLength(paramVector.Vector, settings.SpeedMin, settings.SpeedMax);

                CommonWrites.RequestImpulse(accessor, entityToThrow, velocity, true);

                return true;
            }
        }

        return false;
    }
}