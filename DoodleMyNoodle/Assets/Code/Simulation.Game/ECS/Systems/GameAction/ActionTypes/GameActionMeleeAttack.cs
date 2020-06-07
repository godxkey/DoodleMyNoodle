using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;

public class GameActionMeleeAttack : GameAction
{
    // TODO: add settings on the item itself
    const int DAMAGE = 10;
    const int AP_COST = 1;
    const int RANGE = 1;

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, Entity instigatorPawnController, Entity instigatorPawn)
    {
        return new UseContract(
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
        if (useData.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            int2 instigatorTile = roundToInt(accessor.GetComponentData<FixTranslation>(instigatorPawn).Value).xy;

            // melee attack has a range of RANGE
            if (lengthmanhattan(paramTile.Tile - instigatorTile) > RANGE)
            {
                return;
            }

            if (accessor.GetComponentData<ActionPoints>(instigatorPawn).Value < AP_COST)
            {
                return;
            }

            // reduce instigator AP
            CommonWrites.ModifyStatInt<ActionPoints>(accessor, instigatorPawn, -AP_COST);


            // reduce target health
            NativeList<Entity> victims = new NativeList<Entity>(Allocator.Temp);
            CommonReads.FindEntitiesOnTileWithComponent<Health>(accessor, paramTile.Tile, victims);
            foreach (var entity in victims)
            {
                if (!accessor.HasComponent<Invincible>(entity))
                {
                    CommonWrites.ModifyStatInt<Health>(accessor, entity, -DAMAGE);
                }
            }

            // NB: we might want to queue a 'DamageRequest' in some sort of ProcessDamageSystem instead
        }
    }
}
