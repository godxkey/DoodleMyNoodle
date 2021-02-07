using static fixMath;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using Unity.Mathematics;

public class GameActionDropBombToDetonate : GameAction
{
    fix DROPPING_SPEED = (fix)5f;

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        if (accessor.TryGetComponentData(context.Entity, out ItemSpawnedObjectReference itemSpawnedObjectReference))
        {
            if (itemSpawnedObjectReference.Entity != Entity.Null)
            {
                return new UseContract();
            }
        }

        return new UseContract(new GameActionParameterTile.Description(accessor.GetComponentData<GameActionRangeData>(context.Entity).Value) { });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            // get settings
            if (!accessor.TryGetComponentData(context.Entity, out GameActionObjectReferenceSetting settings))
            {
                Debug.LogWarning($"Item {context.Entity} has no {nameof(GameActionObjectReferenceSetting)} component");
                return false;
            }

            // spawn projectile
            Entity objectInstance = accessor.Instantiate(settings.ObjectPrefab);

            // set projectile data
            fix2 spawnPos = Helpers.GetTileCenter(paramTile.Tile);

            accessor.SetOrAddComponentData(objectInstance, new Velocity() { Value = DROPPING_SPEED * fix2.down });
            accessor.SetOrAddComponentData(objectInstance, new FixTranslation() { Value = spawnPos });
            accessor.SetOrAddComponentData(objectInstance, new PotentialNewTranslation() { Value = spawnPos });

            accessor.SetOrAddComponentData(context.Entity, new ItemSpawnedObjectReference() { Entity = objectInstance });

            return true;
        }
        else
        {
            if (accessor.TryGetComponentData(context.Entity, out ItemSpawnedObjectReference itemSpawnedObjectReference))
            {
                Entity bomb = itemSpawnedObjectReference.Entity;

                if (bomb != Entity.Null)
                {
                    int2 tilePos = Helpers.GetTile(accessor.GetComponentData<FixTranslation>(bomb));

                    int explosionRange = 1;
                    if (accessor.HasComponent<GameActionExplosionRange>(context.Entity))
                    {
                        explosionRange = accessor.GetComponentData<GameActionExplosionRange>(context.Entity).Value;
                    }

                    CommonWrites.RequestExplosionOnTiles(accessor, bomb, tilePos, explosionRange, accessor.GetComponentData<GameActionDamageData>(context.Entity).Value);

                    accessor.DestroyEntity(bomb);

                    accessor.SetOrAddComponentData(context.Entity, new ItemSpawnedObjectReference() { Entity = Entity.Null });

                    return true;
                }
            }
        }

        return false;
    }
}