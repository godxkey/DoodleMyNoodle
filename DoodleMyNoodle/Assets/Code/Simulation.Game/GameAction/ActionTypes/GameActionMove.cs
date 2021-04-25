using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;
using System;

public class GameActionMove : GameAction
{
    public override Type[] GetRequiredSettingTypes() => new Type[]
    {
        typeof(GameActionRangeData)
    };

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
        return 1;
    }

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context)
    {
        int highestRangePossible = ceilToInt(
            accessor.GetComponentData<GameActionRangeData>(context.Item).Value *
            accessor.GetComponentData<ActionPoints>(context.InstigatorPawn).Value);

        UseContract useContract = new UseContract();
        useContract.ParameterTypes = new ParameterDescription[]
        {
            new GameActionParameterTile.Description(highestRangePossible)
            {
                IncludeSelf = false,
                MustBeReachable = true
            }
        };

        return useContract;
    }

    public override bool Use(ISimWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData, ref ResultData resultData)
    {
        if (useData.TryGetParameter(0, out GameActionParameterTile.Data paramTile))
        {
            int2 instigatorTile = Helpers.GetTile(accessor.GetComponentData<FixTranslation>(context.InstigatorPawn));
            fix movePerAP = accessor.GetComponentData<GameActionRangeData>(context.Item).Value;

            NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
            if (!Pathfinding.FindNavigablePath(accessor, instigatorTile, paramTile.Tile, Pathfinding.MAX_PATH_LENGTH, path))
            {
                LogGameActionInfo(context, $"Discarding: cannot find navigable path from { instigatorTile} to { paramTile.Tile}.");
                return false;
            }

            int userAP = accessor.GetComponentData<ActionPoints>(context.InstigatorPawn);

            // Get the last reachable point considering the user's AP
            int lastReachablePathPointIndex = Pathfinding.GetLastPathPointReachableWithinCost(path.AsArray().Slice(), userAP * movePerAP);

            // Remove unreachable points
            path.Resize(lastReachablePathPointIndex + 1, NativeArrayOptions.ClearMemory);

            // find AP cost
            int costToMove = ceilToInt(Pathfinding.CalculateTotalCost(path.Slice()) / movePerAP);

            CommonWrites.ModifyStatInt<ActionPoints>(accessor, context.InstigatorPawn, -costToMove);

            // set destination
            var random = accessor.Random();
            fix2 dest = Helpers.GetTileCenter(path[path.Length - 1]);
            dest += fix2(random.NextFix(fix(-0.075), fix(0.075)), 0);
            accessor.SetOrAddComponentData(context.InstigatorPawn, new Destination() { Value = dest });

            return true;
        }

        return false;
    }
}