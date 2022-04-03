using static fixMath;
using Unity.Entities;
using System;
using CCC.Fix2D;
using CCC.InspectorDisplay;
using UnityEngine;
using System.Collections.Generic;

public class GameActionHealthChange : GameAction<GameActionHealthChange.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public fix Damage;
        public bool IsAutoAttack = false;
        public GameObject OnHealthChangedGameActionPrefab;
        public GameObject OnExtremeReachedGameActionPrefab;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                Damage = Damage,
                IsAutoAttack = IsAutoAttack,
                OnHealthChangedActionEntity = conversionSystem.GetPrimaryEntity(OnHealthChangedGameActionPrefab),
                OnExtremeReachedActionEntity = conversionSystem.GetPrimaryEntity(OnExtremeReachedGameActionPrefab),
            });
        }

        public override void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(OnHealthChangedGameActionPrefab);
            referencedPrefabs.Add(OnExtremeReachedGameActionPrefab);

            base.DeclareReferencedPrefabs(referencedPrefabs);
        }
    }

    public struct Settings : IComponentData
    {
        public fix Damage;
        public bool IsAutoAttack;
        public Entity OnHealthChangedActionEntity;
        public Entity OnExtremeReachedActionEntity;
    }

    protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings)
    {
        return new ExecutionContract();
        // n.b. maybe entity or position in the future ?
    }

    protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
    {
        for (int i = 0; i < input.Context.Targets.Length; i++)
        {
            var target = input.Context.Targets[i];

            HealthChangeRequestData healthChangeRequestData = new HealthChangeRequestData()
            {
                LastPhysicalInstigator = input.Context.LastPhysicalInstigator,
                FirstPhysicalInstigator = input.Context.FirstPhysicalInstigator,
                Target = target,
                Amount = settings.Damage,
                ActionOnHealthChanged = settings.OnHealthChangedActionEntity,
                ActionOnExtremeReached = settings.OnExtremeReachedActionEntity,
                IsAutoAttack = settings.IsAutoAttack
            };

            CommonWrites.RequestHealthChange(input.Accessor, healthChangeRequestData);

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