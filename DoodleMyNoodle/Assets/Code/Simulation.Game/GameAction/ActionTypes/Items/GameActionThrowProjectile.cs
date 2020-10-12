using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;

public class GameActionThrowProjectile : GameAction
{
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
        return accessor.GetComponentData<ItemActionPointCostData>(context.Entity).Value;
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            // get settings
            if (!accessor.TryGetComponentData(context.Entity, out ItemThrowProjectileSettings settings))
            {
                Debug.LogWarning($"Item {context.Entity} has no {nameof(ItemThrowProjectileSettings)} component");
                return false;
            }

            // spawn projectile
            Entity projectileInstance = accessor.Instantiate(settings.ProjectilePrefab);

            // set projectile data
            fix3 spawnPos = Helpers.GetTileCenter(paramTile.Tile);
            fix3 instigatorPos = Helpers.GetTileCenter(Helpers.GetTile(accessor.GetComponentData<FixTranslation>(context.InstigatorPawn)));
            fix3 v = normalize(spawnPos - instigatorPos);

            accessor.SetOrAddComponentData(projectileInstance, new Velocity() { Value = settings.ThrowSpeed * v });
            accessor.SetOrAddComponentData(projectileInstance, new FixTranslation() { Value = Helpers.GetTileCenter(paramTile.Tile) });
            accessor.SetOrAddComponentData(projectileInstance, new PotentialNewTranslation() { Value = Helpers.GetTileCenter(paramTile.Tile) });

            // add 'DamageOnContact' if ItemDamageData found
            if (accessor.HasComponent<ItemDamageData>(context.Entity))
            {
                accessor.SetOrAddComponentData(projectileInstance, new DamageOnContact() { Value = accessor.GetComponentData<ItemDamageData>(context.Entity).Value, DestroySelf = true });
            }

            return true;
        }

        return false;
    }
}
