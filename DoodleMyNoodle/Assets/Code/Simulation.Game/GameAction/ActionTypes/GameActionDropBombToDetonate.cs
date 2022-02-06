using static fixMath;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using Unity.Mathematics;
using CCC.Fix2D;
using System;
using System.Collections.Generic;

public class GameActionDropBombToDetonate : GameAction<GameActionDropBombToDetonate.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public fix Range;
        public int Damage;
        public fix Radius;
        public GameObject Prefab;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                Range = Range,
                Damage = Damage,
                Radius = Radius,
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
        public int Damage;
        public Entity Prefab;
        public fix Radius;
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context, Settings settings)
    {
        if (accessor.TryGetComponent(context.Item, out ItemSpawnedObjectReference itemSpawnedObjectReference))
        {
            if (itemSpawnedObjectReference.Entity != Entity.Null)
            {
                return new UseContract();
            }
        }

        return new UseContract(new GameActionParameterPosition.Description() { MaxRangeFromInstigator = settings.Range });
    }

    public override bool Use(ISimGameWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, List<ResultDataElement> resultData, Settings settings)
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

            accessor.SetOrAddComponent(objectInstance, new FixTranslation() { Value = spawnPos });
            accessor.SetOrAddComponent(context.Item, new ItemSpawnedObjectReference() { Entity = objectInstance });

            return true;
        }
        else
        {
            if (accessor.TryGetComponent(context.Item, out ItemSpawnedObjectReference itemSpawnedObjectReference))
            {
                Entity bomb = itemSpawnedObjectReference.Entity;

                if (bomb != Entity.Null)
                {
                    fix2 bombPos = accessor.GetComponent<FixTranslation>(bomb);

                    CommonWrites.RequestExplosion(accessor, bomb, bombPos, settings.Range, settings.Damage, true);

                    accessor.DestroyEntity(bomb);

                    accessor.SetOrAddComponent(context.Item, new ItemSpawnedObjectReference() { Entity = Entity.Null });

                    return true;
                }
            }
        }

        return false;
    }
}