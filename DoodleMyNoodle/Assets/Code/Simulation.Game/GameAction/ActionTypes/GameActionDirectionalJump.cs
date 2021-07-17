using static fixMath;
using Unity.Entities;
using Unity.Collections;
using CCC.Fix2D;
using System;

public class GameActionDirectionalJump : GameAction<GameActionDirectionalJump.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public fix SpeedMax;
        public fix SpeedMin;
        public fix SpawnExtraDistance;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                SpeedMax = SpeedMax,
                SpeedMin = SpeedMin,
                SpawnExtraDistance = SpawnExtraDistance,
            });
        }
    }

    public struct Settings : IComponentData
    {
        public fix SpeedMax;
        public fix SpeedMin;
        public fix SpawnExtraDistance;
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context, Settings settings)
    {
        return new UseContract(
            new GameActionParameterVector.Description()
            {
                SpeedMin = settings.SpeedMin,
                SpeedMax = settings.SpeedMax
            });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData, Settings settings)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterVector.Data paramVector))
        {
            fix2 velocity = paramVector.Vector;

            velocity = clampLength(velocity, settings.SpeedMin, settings.SpeedMax);

            CommonWrites.RequestImpulse(accessor, context.InstigatorPawn, velocity, ignoreMass: true);

            return true;
        }
        else
        {
            CommonWrites.RequestImpulse(accessor, context.InstigatorPawn, new fix2(0, settings.SpeedMin), ignoreMass: true);

            return true;
        }
    }
}