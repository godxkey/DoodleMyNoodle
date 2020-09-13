using static fixMath;
using Unity.Entities;
using Unity.Collections;

public class GameActionTileExplosion : GameAction
{
    const int EXPLOSIVE_SIZE = 1;

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return new UseContract(
                   new GameActionParameterTile.Description(accessor.GetComponentData<ItemRangeData>(context.Entity).Value) { });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            int damage = accessor.GetComponentData<ItemDamageData>(context.Entity).Value;
            CommonWrites.RequestExplosionOnTiles(accessor, context.InstigatorPawn, paramTile.Tile, EXPLOSIVE_SIZE, damage);

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