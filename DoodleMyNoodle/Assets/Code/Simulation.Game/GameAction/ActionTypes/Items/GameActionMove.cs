using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;
using static fixMath;
using static Unity.Mathematics.math;

public class GameActionMove : GameAction
{
    public override bool IsContextValid(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return accessor.HasComponent<ActionPoints>(context.InstigatorPawn)
            && accessor.HasComponent<FixTranslation>(context.InstigatorPawn);
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        UseContract useContract = new UseContract();
        useContract.ParameterTypes = new ParameterDescription[]
        {
            new GameActionParameterTile.Description(accessor.GetComponentData<ActionPoints>(context.InstigatorPawn))
            {
                IncludeSelf = false,
                MustBeReachable = true
            }
        };

        return useContract;
    }

    public override void Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData)
    {
        if (useData.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            int instigatorAP = accessor.GetComponentData<ActionPoints>(context.InstigatorPawn);
            int2 instigatorTile = roundToInt(accessor.GetComponentData<FixTranslation>(context.InstigatorPawn).Value).xy;

            NativeList<int2> _path = new NativeList<int2>(Allocator.Temp);
            CommonReads.FindNavigablePath(accessor, instigatorTile, paramTile.Tile, Pathfinding.MAX_PATH_COST, _path);

            // Get the last reachable point considering the users' AP
            int lastReachablePathPointIndex = Pathfinding.GetLastPathPointReachableWitingCost(_path.AsArray().Slice(), instigatorAP);

            // Remove unreachable points 
            _path.Resize(lastReachablePathPointIndex + 1, NativeArrayOptions.ClearMemory);

            // find AP cost
            int costToMove = (int)ceil(Pathfinding.CalculateTotalCost(_path.Slice()));

            if (costToMove > instigatorAP)
            {
                Log.Error("Error in logic here! Fix me!");
                return;
            }

            // reduce instigator AP
            CommonWrites.ModifyStatInt<ActionPoints>(accessor, context.InstigatorPawn, -costToMove);

            // set destination
            accessor.SetOrAddComponentData(context.InstigatorPawn, new Destination() { Value = fix3(_path[_path.Length - 1], 0) });
        }
    }
}