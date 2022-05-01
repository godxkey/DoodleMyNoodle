using Unity.Entities;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

public class GameEffectOnDeathAction
{
    public enum ETargetType
    {
        None,
        Killer,
        Victim,
    }

    public class Begin : GameAction<Begin.Settings>
    {
        [Serializable]
        [GameActionSettingAuth(typeof(Settings))]
        public class SettingsAuth : GameActionSettingAuthBase
        {
            public GameObject ActionPrefab;
            public ETargetType Target = ETargetType.Killer;

            public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
            {
                dstManager.AddComponentData(entity, new Settings()
                {
                    OnDeathAction = conversionSystem.GetPrimaryEntity(ActionPrefab),
                    TargetType = Target,
                });
            }

            public override void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
            {
                base.DeclareReferencedPrefabs(referencedPrefabs);
                referencedPrefabs.Add(ActionPrefab);
            }
        }

        public struct Settings : IComponentData
        {
            public Entity OnDeathAction;
            public ETargetType TargetType;
        }

        protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings) => null;

        protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
        {
            // Add damage processor onto effect
            input.Accessor.AddComponentData(input.Context.ActionActor, new OnDeathProcessor()
            {
                FunctionId = GameFunctions.GetId(DamageDealtProcessor)
            });

            // add game effect reference to add on damage dealt
            input.Accessor.AddComponentData(input.Context.ActionActor, new GameEffectOnDeathAction.Settings()
            {
                OnDeathAction = settings.OnDeathAction,
                TargetType = settings.TargetType,
            });

            return true;
        }
    }

    public struct Settings : IComponentData
    {
        public Entity OnDeathAction;
        public ETargetType TargetType;
    }

    [RegisterGameFunction]
    public static readonly GameFunction<GameFunctionOnDeathProcessorArg> DamageDealtProcessor = (ref GameFunctionOnDeathProcessorArg arg) =>
    {
        if (arg.Accessor.TryGetComponent(arg.EffectEntity, out Settings settings))
        {
            switch (settings.TargetType)
            {
                case ETargetType.None:
                    CommonWrites.RequestExecuteGameAction(arg.Accessor, arg.EffectEntity, settings.OnDeathAction);
                    break;
                case ETargetType.Killer:
                    CommonWrites.RequestExecuteGameAction(arg.Accessor, arg.EffectEntity, settings.OnDeathAction, arg.RequestData.InstigatorSet.FirstPhysicalInstigator);
                    break;
                case ETargetType.Victim:
                    CommonWrites.RequestExecuteGameAction(arg.Accessor, arg.EffectEntity, settings.OnDeathAction, arg.RequestData.Target);
                    break;
                default:
                    break;
            }
        }
    };
}
