using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using System.Collections.Generic;

public class GameActionShield : GameAction<GameActionShield.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public fix Range;
        public int TurnDuration;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                Range = Range,
                TurnDuration = TurnDuration
            });
        }
    }

    public struct Settings : IComponentData
    {
        public fix Range;
        public int TurnDuration;
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context, Settings settings)
    {
        if (settings.Range > 0)
        {
            return new UseContract(
                new GameActionParameterEntity.Description()
                {
                    RangeFromInstigator = settings.Range,
                    IncludeSelf = true,
                    RequiresAttackableEntity = true,
                });
        }
        else
        {
            return new UseContract();
        }
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData, List<ResultDataElement> resultData, Settings settings)
    {
        if (useData.TryGetParameter(0, out GameActionParameterEntity.Data paramEntity))
        {
            if (!accessor.Exists(paramEntity.Entity))
            {
                LogGameActionInfo(context, "Target does not exist");
                return false;
            }

            if (!CommonReads.IsInRange(accessor, context.InstigatorPawn, paramEntity.Entity, settings.Range))
            {
                LogGameActionInfo(context, "Target out of range");
                return false;
            }

            ShieldTarget(accessor, context.Item, settings.TurnDuration);
            return true;
        }

        LogGameActionInfo(context, "Wrong parameters");

        return false;
    }

    private void ShieldTarget(ISimWorldReadWriteAccessor accessor, Entity pawn, int duration)
    {
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

