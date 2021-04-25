using static fixMath;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;
using CCC.Fix2D;
using System;

public class GameActionSpawnMinion : GameAction
{
    public override Type[] GetRequiredSettingTypes() => new Type[]
    {
        typeof(GameActionRangeData),
        typeof(GameActionObjectReferenceSetting),
    };

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return new UseContract(
                   new GameActionParameterTile.Description(accessor.GetComponentData<GameActionRangeData>(context.Item).Value)
                   {
                   });
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

            // spawn minion
            Entity objectInstance = accessor.Instantiate(settings.ObjectPrefab);

            accessor.SetOrAddComponentData(objectInstance, new FixTranslation() { Value = Helpers.GetTileCenter(paramTile.Tile) });

            return true;
        }

        return false;
    }
}