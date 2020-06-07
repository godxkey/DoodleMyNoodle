using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using System;
using UnityEngine;
using UnityEngineX;
using Unity.Entities;
using Unity.Collections;

public class GameActionComboAttack : GameAction
{
    // TODO: add settings on the item itself
    const int DAMAGE = 10;
    const int AP_COST_PER_ATTACK = 1;
    const int RANGE = 1;

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, Entity instigatorPawnController, Entity instigatorPawn)
    {
        return new UseContract(
            new GameActionParameterTile.Description()
            {
                Filter = TileFilterFlags.Occupied | TileFilterFlags.NotEmpty,
                RangeFromInstigator = RANGE
            },
            new GameActionParameterTile.Description()
            {
                Filter = TileFilterFlags.Occupied | TileFilterFlags.NotEmpty,
                RangeFromInstigator = RANGE
            });
    }

    public override bool IsInstigatorValid(ISimWorldReadAccessor accessor, Entity instigatorPawnController, Entity instigatorPawn)
    {
        return accessor.HasComponent<ActionPoints>(instigatorPawn)
            && accessor.HasComponent<FixTranslation>(instigatorPawn);
    }

    public override void Use(ISimWorldReadWriteAccessor accessor, Entity instigatorPawnController, Entity instigatorPawn, UseData useData)
    {
        // TODO Find a better way to go through all Parameters (foreach)

        int2 instigatorTile = roundToInt(accessor.GetComponentData<FixTranslation>(instigatorPawn).Value).xy;
        NativeList<Entity> victims = new NativeList<Entity>(Allocator.Temp);

        if (useData.TryGetParameter(0, out GameActionParameterTile.Data firstTile))
        {
            // melee attack has a range of RANGE
            if (lengthmanhattan(firstTile.Tile - instigatorTile) > RANGE)
            {
                return;
            }

            if (accessor.GetComponentData<ActionPoints>(instigatorPawn).Value < AP_COST_PER_ATTACK)
            {
                return;
            }

            // reduce instigator AP
            CommonWrites.ModifyStatInt<ActionPoints>(accessor, instigatorPawn, -AP_COST_PER_ATTACK);

            // find victims
            CommonReads.FindEntitiesOnTileWithComponent<Health>(accessor, firstTile.Tile, victims);
        }

        if (useData.TryGetParameter(1, out GameActionParameterTile.Data secondTile))
        {
            // melee attack has a range of RANGE
            if (lengthmanhattan(secondTile.Tile - instigatorTile) > RANGE)
            {
                return;
            }

            if (accessor.GetComponentData<ActionPoints>(instigatorPawn).Value < AP_COST_PER_ATTACK)
            {
                return;
            }

            // reduce instigator AP
            CommonWrites.ModifyStatInt<ActionPoints>(accessor, instigatorPawn, -AP_COST_PER_ATTACK);

            // find victims
            CommonReads.FindEntitiesOnTileWithComponent<Health>(accessor, secondTile.Tile, victims);
        }

        foreach (Entity entity in victims)
        {
            AttackEntityOnTile(accessor, entity);
        }
    }

    private void AttackEntityOnTile(ISimWorldReadWriteAccessor accessor, Entity entity)
    {
        if (!accessor.HasComponent<Invincible>(entity))
        {
            CommonWrites.ModifyStatInt<Health>(accessor, entity, -DAMAGE);
        }

        // NB: we might want to queue a 'DamageRequest' in some sort of ProcessDamageSystem instead
    }
}
