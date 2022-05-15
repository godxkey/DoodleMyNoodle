using static fixMath;
using Unity.Entities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;
using CCC.Fix2D;
using Unity.Collections;


public class GameEffectAddGameEffectOnDamageDealt
{
    public class Begin : GameAction<Begin.Settings>
    {
        [Serializable]
        [GameActionSettingAuth(typeof(Settings))]
        public class SettingsAuth : GameActionSettingAuthBase
        {
            public GameObject GameEffect;

            public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
            {
                dstManager.AddComponentData(entity, new Settings()
                {
                    GameEffect = conversionSystem.GetPrimaryEntity(GameEffect)
                });
            }

            public override void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
            {
                base.DeclareReferencedPrefabs(referencedPrefabs);
                referencedPrefabs.Add(GameEffect);
            }
        }

        public struct Settings : IComponentData
        {
            public Entity GameEffect;
        }

        protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings) => null;

        protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
        {
            // Add damage processor onto effect
            input.Accessor.AddComponentData(input.Context.ActionInstigator, new DamageDealtProcessor()
            {
                FunctionId = GameFunctions.GetId(DamageDealtProcessor)
            });

            // add game effect reference to add on damage dealt
            input.Accessor.AddComponentData(input.Context.ActionInstigator, new GameEffectAddGameEffectOnDamageDealt.Settings()
            {
                GameEffectPrefab = settings.GameEffect
            });

            return true;
        }
    }

    public struct Settings : IComponentData
    {
        public Entity GameEffectPrefab;
    }

    [RegisterGameFunction]
    public static readonly GameFunction<GameFunctionDamageDealtProcessorArg> DamageDealtProcessor = (ref GameFunctionDamageDealtProcessorArg arg) =>
    {
        if (arg.Accessor.TryGetComponent(arg.EffectEntity, out Settings settings))
        {
            var effectRequests = arg.Accessor.GetSingletonBuffer<AddGameEffectRequest>();

            effectRequests.Add(new AddGameEffectRequest()
            {
                GameEffectPrefab = settings.GameEffectPrefab,
                Target = arg.RequestData.Target,
                Instigator = arg.RequestData.InstigatorSet
            });
        }
    };
}