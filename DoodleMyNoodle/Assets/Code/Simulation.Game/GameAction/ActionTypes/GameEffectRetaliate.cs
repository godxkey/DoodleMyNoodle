using static fixMath;
using Unity.Entities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

public class GameEffectRetaliate
{
    public class GameActionRetaliateBegin : GameAction
    {
        public override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, Entity actionPrefab) => null;

        public override bool Execute(in ExecInputs input, ref ExecOutput output)
        {
            input.Accessor.AddComponentData(input.Context.Action, new DamageProcessor()
            {
                FunctionId = GameFunctions.GetId(DamageProcessor)
            });
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
        }
    };

    public class GameActionRetaliateEnd : GameAction<GameActionRetaliateEnd.Settings>
    {
        [Serializable]
        [GameActionSettingAuth(typeof(Settings))]
        public class SettingsAuth : GameActionSettingAuthBase
        {
            public int AutoAttackCount;
            [Header("Per Mitigated Damage")]
            public float AutoAttackDamageMultiplier;

            public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
            {
                dstManager.AddComponentData(entity, new Settings()
                {
                    AutoAttackCount = AutoAttackCount,
                    AutoAttackDamageMultiplier = (fix)AutoAttackDamageMultiplier
                });
            }
        }

        public struct Settings : IComponentData
        {
            public int AutoAttackCount;
            public fix AutoAttackDamageMultiplier;
        }

        protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings) => null;

        protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
        {
            // add effect
            Log.Info("retaliate: " + input.Accessor.GetComponent<EffectRetaliateDamageCounter>(input.Context.Action));
            return false;
        }
    }
}