using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Entities;
using Unity.Collections;

public class GameActionSwap : GameAction
{
    public override UseContract GetUseContract(ISimWorldReadAccessor _, in UseContext context)
    {
        return new UseContract(
            new GameActionParameterTile.Description(_.GetComponentData<ItemRangeData>(context.Entity).Value)
            {
                IncludeSelf = false,

                // Can swap with any non-static actor
                CustomTileActorPredicate = (tileActor, accessor) => !accessor.HasComponent<StaticTag>(tileActor)
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

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData)
    {
        if (useData.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            fix3 instigatorPos = accessor.GetComponentData<FixTranslation>(context.InstigatorPawn).Value;

            // find target
            NativeList<Entity> victims = new NativeList<Entity>(Allocator.Temp);

            CommonReads.FindTileActors(accessor, paramTile.Tile, victims,
                (tileActor)
                => accessor.HasComponent<FixTranslation>(tileActor)
                && !accessor.HasComponent<StaticTag>(tileActor));

            if (victims.Length > 0)
            {
                // teleport instigator to destination
                CommonWrites.RequestTeleport(accessor, context.InstigatorPawn, paramTile.Tile);
            }

            foreach (Entity entity in victims)
            {
                CommonWrites.RequestTeleport(accessor, entity, instigatorPos);
            }

            return true;
        }

        return false;
    }
}
