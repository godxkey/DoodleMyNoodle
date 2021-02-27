using static fixMath;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using CCC.Fix2D;

public class GameActionDropObject : GameAction
{
    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return new UseContract(
                   new GameActionParameterTile.Description(accessor.GetComponentData<GameActionRangeData>(context.Entity).Value)
                   {
                   });
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

            accessor.SetOrAddComponentData(objectInstance, new FixTranslation(spawnPos));

            return true;
        }

        return false;
    }
}