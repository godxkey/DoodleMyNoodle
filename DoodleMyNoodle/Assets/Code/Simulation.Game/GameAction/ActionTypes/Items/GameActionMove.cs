using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;
using static fixMath;
using static Unity.Mathematics.math;

public class GameActionMove : GameAction
{
    protected override bool CanBeUsedInContextSpecific(ISimWorldReadAccessor accessor, in UseContext context, DebugReason debugReason)
    {
        if (accessor.HasComponent<PathPosition>(context.InstigatorPawn))
        {
            return false;
        }
        
        return true;
    }

    protected override int GetMinimumActionPointCost(ISimWorldReadAccessor accessor, in UseContext context)
    {
        return accessor.GetComponentData<ItemActionPointCostData>(context.Entity).Value;
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        UseContract useContract = new UseContract();
        useContract.ParameterTypes = new ParameterDescription[]
        {
            new GameActionParameterTile.Description(accessor.GetComponentData<ItemRangeData>(context.Entity).Value)
            {
                IncludeSelf = false,
                MustBeReachable = true
            }
        };

        return useContract;
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData)
    {
        if (useData.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            int2 instigatorTile = Helpers.GetTile(accessor.GetComponentData<FixTranslation>(context.InstigatorPawn));
            int moveRange = accessor.GetComponentData<ItemRangeData>(context.Entity).Value;

            NativeList<int2> _path = new NativeList<int2>(Allocator.Temp);
            if(!Pathfinding.FindNavigablePath(accessor, instigatorTile, paramTile.Tile, Pathfinding.MAX_PATH_LENGTH, _path))
            {
                LogGameActionInfo(context, $"Discarding: cannot find navigable path from { instigatorTile} to { paramTile.Tile}.");
                return false;
            }

            // Get the last reachable point considering the user's AP
            int lastReachablePathPointIndex = Pathfinding.GetLastPathPointReachableWithinCost(_path.AsArray().Slice(), moveRange);

            // Remove unreachable points
            _path.Resize(lastReachablePathPointIndex + 1, NativeArrayOptions.ClearMemory);

            // find AP cost
            //int costToMove = (int)ceil(Pathfinding.CalculateTotalCost(_path.Slice()));

            // set destination
            accessor.SetOrAddComponentData(context.InstigatorPawn, new Destination() { Value = Helpers.GetTileCenter(_path[_path.Length - 1]) });

            return true;
        }

        return false;
    }
}