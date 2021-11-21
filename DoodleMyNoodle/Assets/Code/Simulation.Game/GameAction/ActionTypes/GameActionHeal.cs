using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;
using Unity.Entities;
using Unity.Collections;
using System.Collections.Generic;

public class GameActionHeal : GameAction<GameActionHeal.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public fix Range;
        public int MaxHeal;
        public int MinHeal;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                MaxHeal = MaxHeal,
                MinHeal = MinHeal,
                Range = Range,
            });
        }
    }

    public struct Settings : IComponentData
    {
        public int MaxHeal;
        public int MinHeal;
        public fix Range;
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context, Settings settings)
    {
        UseContract useContract = new UseContract();
        useContract.ParameterTypes = new ParameterDescription[]
        {
            new GameActionParameterEntity.Description()
            {
                RangeFromInstigator = settings.Range,
                RequiresAttackableEntity = true,
            }
        };

        return useContract;
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, List<ResultDataElement> resultData, Settings settings)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterEntity.Data paramEntity))
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

            if (!accessor.HasComponent<Health>(paramEntity.Entity))
            {
                LogGameActionInfo(context, "Target has no health");
                return false;
            }

            int maxHP = accessor.GetComponent<MaximumInt<Health>>(paramEntity.Entity).Value;
            int currentHP = accessor.GetComponent<Health>(paramEntity.Entity).Value;
            fix healPowerRatio = 1 - ((fix)currentHP / maxHP); // 0 -> min heal   1 -> max heal
            int heal = roundToInt(lerp(settings.MinHeal, settings.MaxHeal, healPowerRatio));

            CommonWrites.RequestHeal(accessor, context.InstigatorPawn, paramEntity.Entity, heal);

            return true;
        }

        return false;
    }
}
