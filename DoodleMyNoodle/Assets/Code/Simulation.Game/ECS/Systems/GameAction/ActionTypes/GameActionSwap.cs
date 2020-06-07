using Unity.Mathematics;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Entities;
using Unity.Collections;

public class GameActionSwap : GameAction
{
    // TODO: add settings on the item itself
    const int AP_COST = 1;
    const int RANGE = 3;

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
            if (accessor.GetComponentData<ActionPoints>(instigatorPawn).Value < AP_COST)
            {
                return;
            }

            // reduce instigator AP
            CommonWrites.ModifyStatInt<ActionPoints>(accessor, instigatorPawn, -AP_COST);

            fix3 instigatorPos = accessor.GetComponentData<FixTranslation>(instigatorPawn).Value;

            // find target
            NativeList<Entity> victims = new NativeList<Entity>(Allocator.Temp);
            CommonReads.FindEntitiesOnTileWithComponent<ControllableTag>(accessor, paramTile.Tile, victims);
            foreach (Entity entity in victims)
            {
                if (accessor.HasComponent<FixTranslation>(entity))
                {
                    fix3 targetPos = accessor.GetComponentData<FixTranslation>(entity).Value;
                    accessor.SetComponentData(instigatorPawn, new FixTranslation() { Value = targetPos });
                    accessor.SetComponentData(entity, new FixTranslation() { Value = instigatorPos });
                }
            }
        }
    }
}
