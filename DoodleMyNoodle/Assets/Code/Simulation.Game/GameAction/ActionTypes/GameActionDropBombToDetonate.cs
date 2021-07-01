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
        typeof(GameActionSettingEntityReference),
        typeof(GameActionSettingRadius)
    };

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        if (accessor.TryGetComponent(context.Item, out ItemSpawnedObjectReference itemSpawnedObjectReference))
        {
            if (itemSpawnedObjectReference.Entity != Entity.Null)
            {
                return new UseContract();
            }
        }

        return new UseContract(new GameActionParameterTile.Description(accessor.GetComponent<GameActionSettingRange>(context.Item).Value) { });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            // get settings
            if (!accessor.TryGetComponent(context.Item, out GameActionSettingEntityReference settings))
            {
                Debug.LogWarning($"Item {context.Item} has no {nameof(GameActionSettingEntityReference)} component");
                return false;
            }

            // spawn projectile
            Entity objectInstance = accessor.Instantiate(settings.EntityPrefab);

            // set projectile data
            fix2 spawnPos = Helpers.GetTileCenter(paramTile.Tile);

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

                    fix radius = 1;
                    if (accessor.HasComponent<GameActionSettingRadius>(context.Item))
                    {
                        radius = accessor.GetComponent<GameActionSettingRadius>(context.Item).Value;
                    }

                    CommonWrites.RequestExplosion(accessor, bomb, bombPos, radius, accessor.GetComponent<GameActionSettingDamage>(context.Item).Value, true);

                    accessor.DestroyEntity(bomb);

                    accessor.SetOrAddComponent(context.Item, new ItemSpawnedObjectReference() { Entity = Entity.Null });

                    return true;
                }
            }
        }

        return false;
    }
}