using static fixMath;
using Unity.Entities;
using Unity.Collections;
using System;
using UnityEngine;
using System.Collections.Generic;
using CCC.Fix2D;

public class GameActionSpawnPortal : GameAction<GameActionSpawnPortal.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public fix Range;
        public GameObject PortalPrefab;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                Range = Range,
                Prefab = conversionSystem.GetPrimaryEntity(PortalPrefab)
            });
        }

        public override void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
        {
            referencedPrefabs.Add(PortalPrefab);

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
        var param = new GameActionParameterPosition.Description()
        {
            MaxRangeFromInstigator = settings.Range
        };

        return new UseContract(param, param);
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData, Settings settings)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterPosition.Data firstPortalPos))
        {
            if (parameters.TryGetParameter(1, out GameActionParameterPosition.Data secondPortalPos))
            {
                if (!CommonReads.IsInRange(accessor, context.InstigatorPawn, firstPortalPos.Position, settings.Range)
                    || !CommonReads.IsInRange(accessor, context.InstigatorPawn, secondPortalPos.Position, settings.Range))
                {
                    LogGameActionInfo(context, "Target is out of range");
                }

                // spawn portal 1
                Entity firstPortalInstance = accessor.Instantiate(settings.Prefab);

                accessor.SetOrAddComponent(firstPortalInstance, new FixTranslation() { Value = Helpers.GetTileCenter(firstPortalPos.Position) });

                // spawn portal 2
                Entity secondPortalInstance = accessor.Instantiate(settings.Prefab);

                accessor.SetOrAddComponent(secondPortalInstance, new FixTranslation() { Value = Helpers.GetTileCenter(secondPortalPos.Position) });

                accessor.SetOrAddComponent(firstPortalInstance, new Portal() { NextPos = secondPortalPos.Position, NextPortal = secondPortalInstance });
                accessor.SetOrAddComponent(secondPortalInstance, new Portal() { NextPos = firstPortalPos.Position, NextPortal = firstPortalInstance });
            }
        }

        return true;
    }
}