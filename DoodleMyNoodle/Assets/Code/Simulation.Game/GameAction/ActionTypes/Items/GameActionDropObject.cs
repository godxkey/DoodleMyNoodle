using static fixMath;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;

public class GameActionDropObject : GameAction
{
    fix GRAVITY_SPEED = (fix)9.8f;

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return new UseContract(
                   new GameActionParameterTile.Description(accessor.GetComponentData<ItemRangeData>(context.Entity).Value)
                   {
                       MustBeReachable = true,
                       TileFilter = ~TileFlags.Terrain, // All EXCEPT terrain 
                   });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            // get settings
            if (!accessor.TryGetComponentData(context.Entity, out ItemObjectReferenceSetting settings))
            {
                Debug.LogWarning($"Item {context.Entity} has no {nameof(ItemObjectReferenceSetting)} component");
                return false;
            }

            // spawn projectile
            Entity objectInstance = accessor.Instantiate(settings.ObjectPrefab);

            // set projectile data
            fix3 spawnPos = Helpers.GetTileCenter(paramTile.Tile);
            fix3 instigatorPos = accessor.GetComponentData<FixTranslation>(context.InstigatorPawn).Value;

            accessor.SetOrAddComponentData(objectInstance, new Velocity() { Value = GRAVITY_SPEED * fix3.down });
            accessor.SetOrAddComponentData(objectInstance, new FixTranslation() { Value = Helpers.GetTileCenter(paramTile.Tile) });
            accessor.SetOrAddComponentData(objectInstance, new PotentialNewTranslation() { Value = Helpers.GetTileCenter(paramTile.Tile) });

            return true;
        }

        return false;
    }

    protected override bool CanBeUsedInContextSpecific(ISimWorldReadAccessor accessor, in UseContext context, DebugReason debugReason)
    {
        return true;
    }

    protected override int GetMinimumActionPointCost(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return accessor.GetComponentData<ItemActionPointCostData>(context.Entity).Value;
    }
}