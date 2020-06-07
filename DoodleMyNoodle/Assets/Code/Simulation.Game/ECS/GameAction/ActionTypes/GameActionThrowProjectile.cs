using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Entities;
using Unity.Collections;
using UnityEngine;

public class GameActionThrowProjectile : GameAction
{
    // TODO: add settings on the item itself
    const int AP_COST = 1;
    const int RANGE = 1;

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return new UseContract(
            new GameActionParameterTile.Description()
            {
                RangeFromInstigator = RANGE
            });
    }

    public override bool IsInstigatorValid(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return accessor.HasComponent<ActionPoints>(context.InstigatorPawn)
            && accessor.HasComponent<FixTranslation>(context.InstigatorPawn);
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
            if (!accessor.TryGetComponentData(context.ItemEntity, out GameActionThrowProjectileSettings settings))
            {
                Debug.LogWarning($"Item {context.ItemEntity} has no {nameof(GameActionThrowProjectileSettings)} component");
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
