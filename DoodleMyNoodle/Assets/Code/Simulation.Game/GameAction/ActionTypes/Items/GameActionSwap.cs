using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Entities;
using Unity.Collections;

public class GameActionSwap : GameAction
{
    // TODO: add settings on the item itself
    const int AP_COST = 2;
    const int RANGE = 3;

    public override UseContract GetUseContract(ISimWorldReadAccessor _, in UseContext context)
    {
        return new UseContract(
            new GameActionParameterTile.Description(RANGE)
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
        return AP_COST;
    }

    public override void Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData)
    {
        if (useData.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            if (accessor.GetComponentData<ActionPoints>(context.InstigatorPawn).Value < AP_COST)
            {
                return;
            }

            // reduce instigator AP
            CommonWrites.ModifyStatInt<ActionPoints>(accessor, context.InstigatorPawn, -AP_COST);

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
        }
    }
}
