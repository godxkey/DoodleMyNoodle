using Unity.Mathematics;
using static fixMath;
using Unity.Entities;
using Unity.Collections;

public class GameActionComboAttack : GameAction
{
    // TODO: add settings on the item itself
    const int DAMAGE = 10;
    const int AP_COST_PER_ATTACK = 1;
    const int RANGE = 1;

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
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

    public override bool IsInstigatorValid(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return accessor.HasComponent<ActionPoints>(context.InstigatorPawn)
            && accessor.HasComponent<FixTranslation>(context.InstigatorPawn);
    }

    public override void Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters)
    {
        // TODO Find a better way to go through all Parameters (foreach)

        int2 instigatorTile = roundToInt(accessor.GetComponentData<FixTranslation>(context.InstigatorPawn).Value).xy;
        NativeList<Entity> victims = new NativeList<Entity>(Allocator.Temp);

        if (parameters.TryGetParameter(0, out GameActionParameterTile.Data firstTile))
        {
            // melee attack has a range of RANGE
            if (lengthmanhattan(firstTile.Tile - instigatorTile) > RANGE)
            {
                return;
            }

            if (accessor.GetComponentData<ActionPoints>(context.InstigatorPawn).Value < AP_COST_PER_ATTACK)
            {
                return;
            }

            // reduce instigator AP
            CommonWrites.ModifyStatInt<ActionPoints>(accessor, context.InstigatorPawn, -AP_COST_PER_ATTACK);

            // find victims
            CommonReads.FindEntitiesOnTileWithComponent<Health>(accessor, firstTile.Tile, victims);
        }

        if (parameters.TryGetParameter(1, out GameActionParameterTile.Data secondTile))
        {
            // melee attack has a range of RANGE
            if (lengthmanhattan(secondTile.Tile - instigatorTile) > RANGE)
            {
                return;
            }

            if (accessor.GetComponentData<ActionPoints>(context.InstigatorPawn).Value < AP_COST_PER_ATTACK)
            {
                return;
            }

            // reduce instigator AP
            CommonWrites.ModifyStatInt<ActionPoints>(accessor, context.InstigatorPawn, -AP_COST_PER_ATTACK);

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
