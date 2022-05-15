using static fixMath;
using Unity.Entities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;
using CCC.Fix2D;
using Unity.Collections;
using UnityEngine.Serialization;

public class GameEffectRetaliate
{
    public class GameActionBegin : GameAction
    {
        public override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, Entity actionPrefab) => null;

        public override bool Execute(in ExecInputs input, ref ExecOutput output)
        {
            // Add damage processor onto effect
            input.Accessor.AddComponentData(input.Context.ActionInstigator, new DamageReceivedProcessor()
            {
                FunctionId = GameFunctions.GetId(DamageProcessor)
            });
            input.Accessor.AddComponentData(input.Context.ActionInstigator, new EffectRetaliateDamageCounter());
            return true;
        }
    }

    public struct EffectRetaliateDamageCounter : IComponentData
    {
        public fix HighestMitigatedDamage;
    }

    [RegisterGameFunction]
    public static readonly GameFunction<GameFunctionDamageReceivedProcessorArg> DamageProcessor = (ref GameFunctionDamageReceivedProcessorArg arg) =>
    {
        var counter = arg.Accessor.GetComponent<EffectRetaliateDamageCounter>(arg.EffectEntity);

        if (arg.RemainingDamage > 0)
        {
            counter.HighestMitigatedDamage = max(arg.RemainingDamage, counter.HighestMitigatedDamage);
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
            [FormerlySerializedAs("DamagePerMitigatedDamage")]
            public float DamageMultiplier = 1;

            public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
            {
                dstManager.AddComponentData(entity, new Settings()
                {
                    MinDamage = (fix)MinDamage,
                    MaxDamage = (fix)MaxDamage,
                    DamageMultiplier = (fix)DamageMultiplier,
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
            public fix DamageMultiplier;
        }

        protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings) => null;

        protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
        {
            fix highestMitigatedDamage = input.Accessor.GetComponent<EffectRetaliateDamageCounter>(input.Context.ActionInstigator).HighestMitigatedDamage;

            if (highestMitigatedDamage > 0)
            {
                fix totalDamage = clamp(highestMitigatedDamage * settings.DamageMultiplier, settings.MinDamage, settings.MaxDamage);

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

            DamageRequestSettings damageRequest = new DamageRequestSettings()
            {
                DamageAmount = damage,
                InstigatorSet = input.Context.InstigatorSet,
                IsAutoAttack = false,
            };

            CommonWrites.RequestDamage(input.Accessor, damageRequest, input.Context.Targets);
            return true;
        }
    }
}