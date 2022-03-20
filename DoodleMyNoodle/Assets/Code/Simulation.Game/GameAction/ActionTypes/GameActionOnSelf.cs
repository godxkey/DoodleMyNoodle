using static fixMath;
using Unity.Entities;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameActionOnSelf : GameAction<GameActionOnSelf.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public GameObject GameActionPrefab;
        public bool OnFirstInstigator = false;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                ActionEntity = conversionSystem.GetPrimaryEntity(GameActionPrefab.gameObject),
                OnFirstInstigator = OnFirstInstigator
            });
        }

        public override void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            if (GameActionPrefab != null)
            {
                referencedPrefabs.Add(GameActionPrefab);
            }

            base.DeclareReferencedPrefabs(referencedPrefabs);
        }
    }

    public struct Settings : IComponentData
    {
        public Entity ActionEntity;
        public bool OnFirstInstigator;
    }

    protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings)
    {
        return new ExecutionContract();
    }

    protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
    {
        CommonWrites.RequestExecuteGameAction(input.Accessor, input.Context.LastPhysicalInstigator, settings.ActionEntity, input.Context.LastPhysicalInstigator, input.Parameters);

        if (settings.OnFirstInstigator)
        {
            CommonWrites.RequestExecuteGameAction(input.Accessor, input.Context.LastPhysicalInstigator, settings.ActionEntity, input.Context.FirstPhysicalInstigator, input.Parameters);
        }

        return false;
    }
}