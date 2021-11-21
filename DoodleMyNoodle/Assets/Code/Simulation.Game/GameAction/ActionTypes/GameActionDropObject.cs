using static fixMath;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using CCC.Fix2D;
using System;
using System.Collections.Generic;

public class GameActionDropObject : GameAction<GameActionDropObject.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public fix Range;
        public GameObject Prefab;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                Range = Range,
                Prefab = conversionSystem.GetPrimaryEntity(Prefab)
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
        public fix Range;
        public Entity Prefab;
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context, Settings settings)
    {
        return new UseContract(
                   new GameActionParameterPosition.Description()
                   {
                       MaxRangeFromInstigator = settings.Range
                   });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, List<ResultDataElement> resultData, Settings settings)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterPosition.Data paramTile))
        {
            if (!CommonReads.IsInRange(accessor, context.InstigatorPawn, paramTile.Position, settings.Range))
            {
                LogGameActionInfo(context, "Not in range");
            }

            // spawn projectile
            Entity objectInstance = accessor.Instantiate(settings.Prefab);

            // set projectile data
            fix2 spawnPos = Helpers.GetTileCenter(paramTile.Position);

            accessor.SetOrAddComponent(objectInstance, new FixTranslation(spawnPos));

            return true;
        }

        return false;
    }
}