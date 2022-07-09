using Unity.Entities;
using System;
using CCC.Fix2D;
using UnityEngine;
using System.Collections.Generic;

public class GameActionGameEffect : GameAction<GameActionGameEffect.Settings>
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
                GameEffect = conversionSystem.GetPrimaryEntity(GameEffect.gameObject)
            });
        }

        public override void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(GameEffect);

            base.DeclareReferencedPrefabs(referencedPrefabs);
        }
    }

    public struct Settings : IComponentData
    {
        public Entity GameEffect;
    }

    protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings) => null;

    protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
    {
        var effectRequests = input.Accessor.GetSingletonBuffer<SystemRequestAddGameEffect>();

        for (int i = 0; i < input.Context.Targets.Length; i++)
        {
            var target = input.Context.Targets[i];

            effectRequests.Add(new SystemRequestAddGameEffect()
            {
                GameEffectPrefab = settings.GameEffect,
                Target = target,
                Instigator = input.Context.InstigatorSet
            });

            if (input.Accessor.TryGetComponent(target, out FixTranslation targetTranslation))
            {
                output.ResultData.Add(new ResultDataElement()
                {
                    Position = targetTranslation.Value,
                    Entity = target
                });
            }
        }

        return true;
    }
}