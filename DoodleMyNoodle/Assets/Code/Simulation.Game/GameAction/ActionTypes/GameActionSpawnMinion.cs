using static fixMath;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using CCC.Fix2D;
using System;
using System.Collections.Generic;

public class GameActionSpawnMinion : GameAction<GameActionSpawnMinion.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public fix Range;
        public GameObject MinionPrefab;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                Range = Range,
                Prefab = conversionSystem.GetPrimaryEntity(MinionPrefab)
            });
        }

        public override void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(MinionPrefab);

            base.DeclareReferencedPrefabs(referencedPrefabs);
        }
    }

    public struct Settings : IComponentData
    {
        public fix Range;
        public Entity Prefab;
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context, Settings settings)
    {
        return new UseContract(
                   new GameActionParameterPosition.Description()
                   {
                       MaxRangeFromInstigator = settings.Range,
                   });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData, Settings settings)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterPosition.Data paramPosition))
        {
            if (!CommonReads.IsInRange(accessor, context.InstigatorPawn, paramPosition.Position, settings.Range))
            {
                LogGameActionInfo(context, "Target is out of range");
            }

            // spawn minion
            Entity minionInstance = accessor.Instantiate(settings.Prefab);

            accessor.SetOrAddComponent(minionInstance, new FixTranslation() { Value = Helpers.GetTileCenter(paramPosition.Position) });

            // Set minion team
            if (accessor.HasComponent<DefaultControllerTeam>(minionInstance))
            {
                int instigatorTeam = accessor.GetComponent<Team>(context.InstigatorPawnController).Value;
                accessor.SetComponent(minionInstance, new DefaultControllerTeam() { Value = instigatorTeam });
            }

            return true;
        }

        return false;
    }
}