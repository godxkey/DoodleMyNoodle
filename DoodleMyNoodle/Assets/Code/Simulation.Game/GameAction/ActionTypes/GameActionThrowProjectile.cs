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

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            // get settings
            if (!accessor.TryGetComponentData(context.Entity, out GameActionThrowProjectileSettings settings))
            {
                Debug.LogWarning($"Item {context.Entity} has no {nameof(GameActionThrowProjectileSettings)} component");
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
            if (accessor.HasComponent<GameActionDamageData>(context.Entity))
            {
                accessor.SetOrAddComponentData(projectileInstance, new DamageOnContact() { Value = accessor.GetComponentData<GameActionDamageData>(context.Entity).Value, DestroySelf = true });
            }

            return true;
        }

        return false;
    }
}