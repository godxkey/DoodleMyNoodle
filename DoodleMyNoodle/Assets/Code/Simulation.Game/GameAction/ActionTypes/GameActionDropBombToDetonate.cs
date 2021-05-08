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
        typeof(GameActionSettingRange),
        typeof(GameActionSettingDamage),
        typeof(GameActionSettingObjectReference),
        typeof(GameActionSettingRadius)
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

        return new UseContract(new GameActionParameterTile.Description(accessor.GetComponentData<GameActionSettingRange>(context.Item).Value) { });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            // get settings
            if (!accessor.TryGetComponentData(context.Item, out GameActionSettingObjectReference settings))
            {
                Debug.LogWarning($"Item {context.Item} has no {nameof(GameActionSettingObjectReference)} component");
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
                    fix2 bombPos = accessor.GetComponentData<FixTranslation>(bomb);

                    fix radius = 1;
                    if (accessor.HasComponent<GameActionSettingRadius>(context.Item))
                    {
                        radius = accessor.GetComponentData<GameActionSettingRadius>(context.Item).Value;
                    }

                    CommonWrites.RequestExplosion(accessor, bomb, bombPos, radius, accessor.GetComponentData<GameActionSettingDamage>(context.Item).Value);

                    accessor.DestroyEntity(bomb);

                    accessor.SetOrAddComponentData(context.Item, new ItemSpawnedObjectReference() { Entity = Entity.Null });

                    return true;
                }
            }
        }

        return false;
    }
}