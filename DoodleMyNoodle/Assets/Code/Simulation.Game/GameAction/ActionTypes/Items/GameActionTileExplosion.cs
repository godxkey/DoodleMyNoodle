using static fixMath;
using Unity.Entities;
using Unity.Collections;

public class GameActionTileExplosion : GameAction
{
    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return new UseContract(
                   new GameActionParameterTile.Description(accessor.GetComponentData<GameActionRangeData>(context.Entity).Value) { });
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters parameters, ref ResultData resultData)
    {
        if (parameters.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            int damage = accessor.GetComponentData<GameActionDamageData>(context.Entity).Value;

            int explosionRange = 1;
            if (accessor.HasComponent<GameActionExplosionRange>(context.Entity))
            {
                explosionRange = accessor.GetComponentData<GameActionExplosionRange>(context.Entity).Value;
            }

            CommonWrites.RequestExplosionOnTiles(accessor, context.InstigatorPawn, paramTile.Tile, explosionRange, damage);

            return true;   
        }

        return false;
    }
}