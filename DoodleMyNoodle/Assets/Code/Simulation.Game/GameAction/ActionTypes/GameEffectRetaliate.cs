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
            public GameObject ProjectilePrefab;
            public Vector2 ThrowVelocity;
            public Vector2 SpawnOffset;
            public float MinDamage = 1;
            public float MaxDamage = 15;
            public float DamagePerMitigatedDamage = 1;

            public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
            {
                dstManager.AddComponentData(entity, new Settings()
                {
                    MinDamage = (fix)MinDamage,
                    MaxDamage = (fix)MaxDamage,
                    DamagePerMitigatedDamage = (fix)DamagePerMitigatedDamage,
                    ProjectilePrefab = conversionSystem.GetPrimaryEntity(ProjectilePrefab),
                    ThrowVelocity = (fix2)ThrowVelocity,
                    SpawnOffset = (fix2)SpawnOffset,
                });
            }

            public override void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
            {
                base.DeclareReferencedPrefabs(referencedPrefabs);
                referencedPrefabs.Add(ProjectilePrefab);
            }
        }

        public struct Settings : IComponentData
        {
            public Entity ProjectilePrefab;
            public fix2 ThrowVelocity;
            public fix2 SpawnOffset;
            public fix MinDamage;
            public fix MaxDamage;
            public fix DamagePerMitigatedDamage;
        }

        protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings) => null;

        protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
        {
            fix mitigatedDamage = input.Accessor.GetComponent<EffectRetaliateDamageCounter>(input.Context.ActionInstigatorActor).MitigatedDamage;

            if (mitigatedDamage > 0)
            {
                fix totalDamage = clamp(mitigatedDamage * settings.DamagePerMitigatedDamage, settings.MinDamage, settings.MaxDamage);

                var fireSettings = FireProjectileSettings.Default;
                fireSettings.SpawnOffset = settings.SpawnOffset;
                Entity projectile = CommonWrites.FireProjectile(input.Accessor, input.Context.InstigatorSet, settings.ProjectilePrefab, settings.ThrowVelocity, fireSettings);

                input.Accessor.AddComponentData<ShieldDamage>(projectile, totalDamage);
            }


            return true;
        }
    }

    public struct ShieldDamage : IComponentData
    {
        public fix Value;

        public static implicit operator fix(ShieldDamage val) => val.Value;
        public static implicit operator ShieldDamage(fix val) => new ShieldDamage() { Value = val };
    }

    public class GameActionShieldDamage : GameAction
    {
        public override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, Entity entity) => null;

        public override bool Execute(in ExecInputs input, ref ExecOutput output)
        {
            fix damage = input.Accessor.GetComponent<ShieldDamage>(input.Context.LastPhysicalInstigator);
            CommonWrites.RequestDamage(input.Accessor, input.Context.LastPhysicalInstigator, input.Context.Targets, damage);
            return true;
        }
    }
}