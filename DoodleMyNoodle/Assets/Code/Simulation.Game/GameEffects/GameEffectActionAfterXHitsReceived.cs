using Unity.Entities;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameEffectActionAfterXHitsReceived
{
    public enum TargetType
    {
        HitVictim,
        HitInstigator
    }

    public struct EffectData : IComponentData
    {
        public Entity GameActionPrefab;
        public int RequiredHits;
        public int HitCounter;
        public TargetType TargetType;
        public Entity OnlyHitsFromThisActor;
    }

    public class Begin : GameAction<Begin.Settings>
    {
        [Serializable]
        [GameActionSettingAuth(typeof(Settings))]
        public class SettingsAuth : GameActionSettingAuthBase
        {
            public int RequiredHits;
            public GameObject GameAction;
            public TargetType TargetType = TargetType.HitVictim;
            public bool OnlyHitsFromEffectInstigator;

            public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
            {
                dstManager.AddComponentData(entity, new Settings()
                {
                    GameAction = conversionSystem.GetPrimaryEntity(GameAction),
                    RequiredHits = RequiredHits,
                    TargetType = TargetType,
                    OnlyHitsFromEffectInstigator = OnlyHitsFromEffectInstigator,
                });
            }

            public override void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
            {
                base.DeclareReferencedPrefabs(referencedPrefabs);
                referencedPrefabs.Add(GameAction);
            }
        }

        public struct Settings : IComponentData
        {
            public Entity GameAction;
            public int RequiredHits;
            public TargetType TargetType;
            public bool OnlyHitsFromEffectInstigator;
        }

        protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings) => null;

        protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
        {
            // Add damage processor onto effect
            input.Accessor.AddComponentData(input.Context.ActionInstigator, new DamageReceivedProcessor()
            {
                FunctionId = GameFunctions.GetId(DamageReceivedProcessor)
            });

            // add game effect reference to add on damage dealt
            input.Accessor.AddComponentData(input.Context.ActionInstigator, new EffectData()
            {
                GameActionPrefab = settings.GameAction,
                HitCounter = 0,
                RequiredHits = settings.RequiredHits,
                TargetType = settings.TargetType,
                OnlyHitsFromThisActor = settings.OnlyHitsFromEffectInstigator
                    ? CommonReads.GetOwnerActor(input.Accessor, input.Accessor.GetComponent<FirstInstigator>(input.Context.ActionInstigator))
                    : Entity.Null,
            });

            return true;
        }
    }

    [RegisterGameFunction]
    public static readonly GameFunction<GameFunctionDamageReceivedProcessorArg> DamageReceivedProcessor = (ref GameFunctionDamageReceivedProcessorArg arg) =>
    {
        if (arg.Accessor.TryGetComponent(arg.EffectEntity, out EffectData data))
        {
            Entity firstInstigatorActor = CommonReads.GetFirstInstigatorActor(arg.Accessor, arg.RequestData.Instigator);

            if (data.OnlyHitsFromThisActor != Entity.Null && data.OnlyHitsFromThisActor != firstInstigatorActor)
            {
                return;
            }

            data.HitCounter++;

            arg.Accessor.SetComponent(arg.EffectEntity, data);

            if (data.HitCounter == data.RequiredHits)
            {
                Entity target = Entity.Null;
                switch (data.TargetType)
                {
                    case TargetType.HitVictim:
                        target = arg.RequestData.Target;
                        break;
                    case TargetType.HitInstigator:
                        target = firstInstigatorActor;
                        break;
                }

                CommonWrites.RequestExecuteGameAction(arg.Accessor, arg.EffectEntity, data.GameActionPrefab, target);
            }

        }
    };
}
