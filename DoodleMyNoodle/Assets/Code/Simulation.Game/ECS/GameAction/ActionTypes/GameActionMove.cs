using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class GameActionMove : GameAction
{
    public override bool IsInstigatorValid(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return accessor.HasComponent<ActionPoints>(context.InstigatorPawn)
            && accessor.HasComponent<FixTranslation>(context.InstigatorPawn);
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        UseContract useContract = new UseContract();
        useContract.ParameterTypes = new ParameterDescription[]
        {
            new GameActionParameterTile.Description()
            {
                RangeFromInstigator = accessor.GetComponentData<ActionPoints>(context.InstigatorPawn).Value,
                Filter = TileFilterFlags.Navigable | TileFilterFlags.Inoccupied
            }
        };

        return useContract;
    }

    public override void Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData)
    {
        if (useData.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            int instigatorAP = accessor.GetComponentData<ActionPoints>(context.InstigatorPawn).Value;
            int2 instigatorTile = roundToInt(accessor.GetComponentData<FixTranslation>(context.InstigatorPawn).Value).xy;

            NativeList<int2> _path = new NativeList<int2>(Allocator.Temp);
            CommonReads.FindNavigablePath(accessor, instigatorTile, paramTile.Tile, Pathfinding.MAX_PATH_COST, _path);
            int costToMove = _path.Length - 1;

            if (costToMove > instigatorAP)
            {
                return;
            }

            // reduce instigator AP
            CommonWrites.ModifyStatInt<ActionPoints>(accessor, context.InstigatorPawn, -costToMove);

            // set destination
            accessor.SetOrAddComponentData(context.InstigatorPawn, new Destination() { Value = fix3(paramTile.Tile, 0) });
        }
    }
}