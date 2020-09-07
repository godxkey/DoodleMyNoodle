using Unity.Mathematics;
using static fixMath;
using Unity.Entities;
using Unity.Collections;

public class GameActionComboAttack : GameAction
{
    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        var param = new GameActionParameterTile.Description(accessor.GetComponentData<ItemRangeData>(context.Entity).Value)
        {
            IncludeSelf = false,
            RequiresAttackableEntity = true
        };

        return new UseContract(param, param);
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
        int2 instigatorTile = Helpers.GetTile(accessor.GetComponentData<FixTranslation>(context.InstigatorPawn));
        NativeList<Entity> victims = new NativeList<Entity>(Allocator.Temp);

        if (parameters.TryGetParameter(0, out GameActionParameterTile.Data firstTile))
        {
            // melee attack has a range
            if (lengthmanhattan(firstTile.Tile - instigatorTile) > accessor.GetComponentData<ItemRangeData>(context.Entity).Value)
            {
                return false;
            }

            // find victims
            CommonReads.FindTileActorsWithComponents<Health>(accessor, firstTile.Tile, victims);
        }

        if (parameters.TryGetParameter(1, out GameActionParameterTile.Data secondTile))
        {
            // melee attack has a range of RANGE
            if (lengthmanhattan(secondTile.Tile - instigatorTile) > accessor.GetComponentData<ItemRangeData>(context.Entity).Value)
            {
                return false;
            }

            // find victims
            CommonReads.FindTileActorsWithComponents<Health>(accessor, secondTile.Tile, victims);
        }

        foreach (Entity entity in victims)
        {
            AttackEntityOnTile(accessor, context, entity);
        }

        if (victims.Length > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void AttackEntityOnTile(ISimWorldReadWriteAccessor accessor, UseContext context, Entity entity)
    {
        CommonWrites.RequestDamageOnTarget(accessor, context.InstigatorPawn, entity, accessor.GetComponentData<ItemDamageData>(context.Entity).Value);
    }

}
