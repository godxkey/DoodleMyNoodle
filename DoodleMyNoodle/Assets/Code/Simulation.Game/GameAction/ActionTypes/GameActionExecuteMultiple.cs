using static fixMath;
using Unity.Entities;
using System;
using System.Collections.Generic;
using UnityEngine;

public class GameActionExecuteMultiple : GameAction<GameActionExecuteMultiple.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public List<GameObject> GameActionPrefabs = new List<GameObject>();

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            for (int i = 0; i < GameActionPrefabs.Count; i++)
            {
                var actionPrefab = GameActionPrefabs[i];
                if (actionPrefab == null)
                    continue;

                var buffer = dstManager.GetOrAddBuffer<ActionsBufferElement>(entity);
                buffer.Add(new ActionsBufferElement()
                {
                    ActionEntity = conversionSystem.GetPrimaryEntity(actionPrefab.gameObject)
                });
            }

            dstManager.AddComponentData(entity, new Settings() { });
        }

        public override void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            foreach (var prefab in GameActionPrefabs)
            {
                if (prefab == null)
                    continue;

                referencedPrefabs.Add(prefab);
            }

            base.DeclareReferencedPrefabs(referencedPrefabs);
        }
    }

    public struct Settings : IComponentData 
    {
        public bool uselessVariable; // to prevent Settings emtpy bug
    }

    protected override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, ref Settings settings)
    {
        return new ExecutionContract();
    }

    protected override bool Execute(in ExecInputs input, ref ExecOutput output, ref Settings settings)
    {
        var buffer = input.Accessor.GetBufferReadOnly<ActionsBufferElement>(input.Context.Action);
        foreach (ActionsBufferElement gameAction in buffer)
        {
            CommonWrites.RequestExecuteGameAction(input.Accessor, input.Context.LastPhysicalInstigator, gameAction.ActionEntity, input.Context.Targets, input.Parameters);
        }

        return true;
    }
}