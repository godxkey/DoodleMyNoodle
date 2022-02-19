using Unity.Entities;
using UnityEngine;
using CCC.Fix2D;
using System;
using System.Collections.Generic;

public class GameActionSpawn : GameAction<GameActionSpawn.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public Vector2 PawnOffset;
        public GameObject Prefab;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                Prefab = conversionSystem.GetPrimaryEntity(Prefab),
                PawnOffset = (fix2)PawnOffset
            });
        }

        public override void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(Prefab);

            base.DeclareReferencedPrefabs(referencedPrefabs);
        }
    }

    public struct Settings : IComponentData
    {
        public Entity Prefab;
        public fix2 PawnOffset;
    }

    public override ExecutionContract GetExecutionContract(ISimWorldReadAccessor accessor, Settings settings)
    {
        return new ExecutionContract(
            new GameActionParameterPosition.Description()
            {
                MaxRangeFromInstigator = 9999,
            });
    }

    public override bool Use(ISimGameWorldReadWriteAccessor accessor, in ExecutionContext context, UseParameters parameters, List<ResultDataElement> resultData, Settings settings)
    {
        fix2 spawnPosition = accessor.GetComponent<FixTranslation>(context.LastPhysicalInstigator) + settings.PawnOffset;

        if (parameters != null && parameters.TryGetParameter(0, out GameActionParameterPosition.Data paramPos))
        {
            spawnPosition = paramPos.Position;
        }

        // spawn
        Entity instance = accessor.Instantiate(settings.Prefab);

        accessor.SetOrAddComponent(instance, new FixTranslation(spawnPosition));
        accessor.SetOrAddComponent(instance, new FirstInstigator() { Value = context.FirstPhysicalInstigator });

        return true;
    }
}
