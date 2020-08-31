using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;

public class GameActionThrowProjectile : GameAction
{
    // TODO: add settings on the item itself
    const int AP_COST = 3;
    const int RANGE = 1;

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return new UseContract(
            new GameActionParameterTile.Description(rangeFromInstigator: 1)
            {
                IncludeSelf = false,
                TileFilter = ~TileFlags.Terrain, // All EXCEPT terrain 
            });
    }

    protected override bool CanBeUsedInContextSpecific(ISimWorldReadAccessor accessor, in UseContext context, DebugReason debugReason)
    {
        return true;
    }

    protected override int GetMinimumActionPointCost(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return AP_COST;
    }

    public override void Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            if (accessor.GetComponentData<ActionPoints>(context.InstigatorPawn).Value < AP_COST)
            {
                return;
            }

            // reduce instigator AP
            CommonWrites.ModifyStatInt<ActionPoints>(accessor, context.InstigatorPawn, -AP_COST);

            // get settings
            if (!accessor.TryGetComponentData(context.Entity, out GameActionThrowProjectileSettings settings))
            {
                Debug.LogWarning($"Item {context.Entity} has no {nameof(GameActionThrowProjectileSettings)} component");
                return;
            }

            // spawn projectile
            Entity projectileInstance = accessor.Instantiate(settings.ProjectilePrefab);

            // set projectile data
            fix3 spawnPos = Helpers.GetTileCenter(paramTile.Tile);
            fix3 instigatorPos = accessor.GetComponentData<FixTranslation>(context.InstigatorPawn).Value;
            fix3 v = normalize(spawnPos - instigatorPos);

            accessor.SetOrAddComponentData(projectileInstance, new Velocity() { Value = settings.ThrowSpeed * v });
            accessor.SetOrAddComponentData(projectileInstance, new FixTranslation() { Value = Helpers.GetTileCenter(paramTile.Tile) });
            accessor.SetOrAddComponentData(projectileInstance, new PotentialNewTranslation() { Value = Helpers.GetTileCenter(paramTile.Tile) });

        }
    }
}
