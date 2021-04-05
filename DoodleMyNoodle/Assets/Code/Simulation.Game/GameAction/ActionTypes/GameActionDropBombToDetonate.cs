using static fixMath;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using Unity.Mathematics;
using CCC.Fix2D;
using System;

public class GameActionDropBombToDetonate : GameAction
{
    public override Type[] GetRequiredSettingTypes() => new Type[]
    {
        typeof(GameActionRangeData),
        typeof(GameActionDamageData),
        typeof(GameActionObjectReferenceSetting),
        typeof(GameActionExplosionRange)
    };

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        if (accessor.TryGetComponentData(context.Item, out ItemSpawnedObjectReference itemSpawnedObjectReference))
        {
            if (itemSpawnedObjectReference.Entity != Entity.Null)
            {
                return new UseContract();
            }
        }

        return new UseContract(new GameActionParameterTile.Description(accessor.GetComponentData<GameActionRangeData>(context.Item).Value) { });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            // get settings
            if (!accessor.TryGetComponentData(context.Item, out GameActionObjectReferenceSetting settings))
            {
                Debug.LogWarning($"Item {context.Item} has no {nameof(GameActionObjectReferenceSetting)} component");
                return false;
            }

            // spawn projectile
            Entity objectInstance = accessor.Instantiate(settings.ObjectPrefab);

            // set projectile data
            fix2 spawnPos = Helpers.GetTileCenter(paramTile.Tile);

            accessor.SetOrAddComponentData(objectInstance, new FixTranslation() { Value = spawnPos });
            accessor.SetOrAddComponentData(context.Item, new ItemSpawnedObjectReference() { Entity = objectInstance });

            return true;
        }
        else
        {
            if (accessor.TryGetComponentData(context.Item, out ItemSpawnedObjectReference itemSpawnedObjectReference))
            {
                Entity bomb = itemSpawnedObjectReference.Entity;

                if (bomb != Entity.Null)
                {
                    int2 tilePos = Helpers.GetTile(accessor.GetComponentData<FixTranslation>(bomb));

                    int explosionRange = 1;
                    if (accessor.HasComponent<GameActionExplosionRange>(context.Item))
                    {
                        explosionRange = accessor.GetComponentData<GameActionExplosionRange>(context.Item).Value;
                    }

                    CommonWrites.RequestExplosionOnTiles(accessor, bomb, tilePos, explosionRange, accessor.GetComponentData<GameActionDamageData>(context.Item).Value);

                    accessor.DestroyEntity(bomb);

                    accessor.SetOrAddComponentData(context.Item, new ItemSpawnedObjectReference() { Entity = Entity.Null });

                    return true;
                }
            }
        }

        return false;
    }
}