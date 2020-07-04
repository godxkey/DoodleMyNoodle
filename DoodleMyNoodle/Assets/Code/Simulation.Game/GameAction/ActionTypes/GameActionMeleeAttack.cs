using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;

public class GameActionMeleeAttack : GameAction
{
    // TODO: add settings on the item itself
    const int DAMAGE = 2;

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return new UseContract(
            new GameActionParameterTile.Description()
            {
                Filter = TileFilterFlags.Occupied | TileFilterFlags.NotEmpty,
                RangeFromInstigator = accessor.GetComponentData<ItemRangeData>(context.ItemEntity).Value
            });
    }

    public override bool IsContextValid(ISimWorldReadAccessor accessor, in UseContext context)
    {
        // Cooldown
        if (accessor.HasComponent<ItemCooldownCounter>(context.ItemEntity) && accessor.GetComponentData<ItemCooldownCounter>(context.ItemEntity).Value > 0)
        {
            return false;
        }

        return accessor.HasComponent<ActionPoints>(context.InstigatorPawn)
            && accessor.HasComponent<FixTranslation>(context.InstigatorPawn);
    }

    public override void Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData)
    {
        if (useData.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            int2 instigatorTile = roundToInt(accessor.GetComponentData<FixTranslation>(context.InstigatorPawn).Value).xy;

            // melee attack has a range of RANGE
            if (lengthmanhattan(paramTile.Tile - instigatorTile) > accessor.GetComponentData<ItemRangeData>(context.ItemEntity).Value)
            {
                return;
            }

            accessor.SetOrAddComponentData(context.ItemEntity, new ItemCooldownCounter() { Value = accessor.GetComponentData<ItemCooldownData>(context.ItemEntity).Value });

            // reduce target health
            NativeList<Entity> victims = new NativeList<Entity>(Allocator.Temp);
            CommonReads.FindEntitiesOnTileWithComponent<Health>(accessor, paramTile.Tile, victims);
            foreach (Entity entity in victims)
            {
                CommonWrites.RequestDamageOnTarget(accessor, context.InstigatorPawn, entity, accessor.GetComponentData<ItemDamageData>(context.ItemEntity).Value);
            }
        }
    }
}
