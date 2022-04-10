using static fixMath;
using Unity.Entities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;
using CCC.Fix2D;
using Unity.Collections;

public class GameEffectRetaliate
{
    public class GameActionBegin : GameAction
    {
        public override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, Entity actionPrefab) => null;

        public override bool Execute(in ExecInputs input, ref ExecOutput output)
        {
            // Add damage processor onto effect
            input.Accessor.AddComponentData(input.Context.ActionInstigatorActor, new DamageProcessor()
            {
                FunctionId = GameFunctions.GetId(DamageProcessor)
            });
            input.Accessor.AddComponentData(input.Context.ActionInstigatorActor, new EffectRetaliateDamageCounter());
            return true;
        }
    }

    public struct EffectRetaliateDamageCounter : IComponentData
    {
        public fix MitigatedDamage;
    }

    [RegisterGameFunction]
    public static readonly GameFunction<GameFunctionDamageProcessorArg> DamageProcessor = (ref GameFunctionDamageProcessorArg arg) =>
    {
        var counter = arg.Accessor.GetComponent<EffectRetaliateDamageCounter>(arg.EffectEntity);

        if (arg.RemainingDamage > 0)
        {
            counter.MitigatedDamage += arg.RemainingDamage;
            arg.RemainingDamage = 0;
            arg.Accessor.SetComponent(arg.EffectEntity, counter);
        }
    };

    public class GameActionEnd : GameAction<GameActionEnd.Settings>
    {
        [Serializable]
        [GameActionSettingAuth(typeof(Settings))]
        public class SettingsAuth : GameActionSettingAuthBase
        {
            public float DamageRadius = 3;
            public float MinDamage = 1;
            public float MaxDamage = 15;
            public float DamagePerMitigatedDamage = 1;

            public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
            {
                dstManager.AddComponentData(entity, new Settings()
                {
                    DamageRadius = (fix)DamageRadius,
                    MinDamage = (fix)MinDamage,
                    MaxDamage = (fix)MaxDamage,
                    DamagePerMitigatedDamage = (fix)DamagePerMitigatedDamage,
                });
            }
        }

        public struct Settings : IComponentData
        {
            public fix DamageRadius;
            public fix MinDamage;
            public fix MaxDamage;
            public fix DamagePerMitigatedDamage;
        }

        protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings) => null;

        protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
        {
            // add effect
            ActorFilterInfo instigatorFilterInfo = CommonReads.GetActorFilterInfo(input.Accessor, input.Context.FirstPhysicalInstigator);
            fix2 instigatorPos = input.Accessor.GetComponent<FixTranslation>(input.Context.LastPhysicalInstigator);
            NativeList<Entity> actorsInRange = CommonReads.Physics.OverlapCircle(input.Accessor, instigatorPos, settings.DamageRadius).ToEntityList();

            CommonReads.FilterActors(input.Accessor, actorsInRange, instigatorFilterInfo, ActorFilter.Enemies);

            fix mitigatedDamage = input.Accessor.GetComponent<EffectRetaliateDamageCounter>(input.Context.ActionInstigatorActor).MitigatedDamage;
            fix totalDamage = clamp(mitigatedDamage * settings.DamagePerMitigatedDamage, settings.MinDamage, settings.MaxDamage);

            foreach (var target in actorsInRange)
            {
                CommonWrites.RequestDamage(input.Accessor, input.Context.LastPhysicalInstigator, target, totalDamage, Entity.Null, Entity.Null);
            }
            return true;
        }
    }
}