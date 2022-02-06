using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using CCC.Fix2D;
using System;
using System.Collections.Generic;

public class GameActionMove : GameAction<GameActionMove.Settings>
{
    [Serializable]
    [GameActionSettingAuth(typeof(Settings))]
    public class SettingsAuth : GameActionSettingAuthBase
    {
        public fix RangePerAP;

        public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Settings()
            {
                RangePerAP = RangePerAP,
            });
        }
    }

    public struct Settings : IComponentData
    {
        public fix RangePerAP;
    }

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

    public override UseContract GetUseContract(ISimWorldReadAccessor accessor, in UseContext context, Settings settings)
    {
        int highestRangePossible = ceilToInt(
            settings.RangePerAP *
            accessor.GetComponent<ActionPoints>(context.InstigatorPawn).Value);

        UseContract useContract = new UseContract();
        //useContract.ParameterTypes = new ParameterDescription[]
        //{
        //    new GameActionParameterTile.Description(highestRangePossible)
        //    {
        //        IncludeSelf = false,
        //        MustBeReachable = true,
        //        TileFilter = TileFlags.Ladder
        //    }
        //};

        return useContract;
    }

    public override bool Use(ISimGameWorldReadWriteAccessor accessor, in UseContext context, UseParameters useData, List<ResultDataElement> resultData, Settings settings)
    {
        //int2 destinationTile;
        //fix2 destinationPosition;

        //if (useData.TryGetParameter(0, out GameActionParameterTile.Data paramTile, warnIfFailed: false))
        //{
        //    destinationTile = paramTile.Tile;
        //    destinationPosition = Helpers.GetTileCenter(destinationTile);
        //}
        //else if (useData.TryGetParameter(0, out GameActionParameterPosition.Data paramPosition))
        //{
        //    destinationTile = Helpers.GetTile(paramPosition.Position);
        //    destinationPosition = paramPosition.Position;
        //    destinationPosition.y = Helpers.GetTileCenter(destinationTile).y; // make sure Y is at center
        //}
        //else
        //{
        //    return false;
        //}

        //int2 instigatorTile = Helpers.GetTile(accessor.GetComponent<FixTranslation>(context.InstigatorPawn));

        //NativeList<int2> path = new NativeList<int2>(Allocator.Temp);
        //if (!Pathfinding.FindNavigablePath(accessor, instigatorTile, destinationTile, Pathfinding.MAX_PATH_LENGTH, path))
        //{
        //    LogGameActionInfo(context, $"Discarding: cannot find navigable path from {instigatorTile} to {destinationTile}.");
        //    return false;
        //}

        //fix userAP = accessor.GetComponent<ActionPoints>(context.InstigatorPawn);

        //// Get the last reachable point considering the user's AP
        //int lastReachablePathPointIndex = Pathfinding.GetLastPathPointReachableWithinCost(path.AsArray().Slice(), userAP * settings.RangePerAP);

        //// Remove unreachable points
        //if (path.Length > lastReachablePathPointIndex + 1)
        //{
        //    path.Resize(lastReachablePathPointIndex + 1, NativeArrayOptions.ClearMemory);

        //    destinationPosition = Helpers.GetTileCenter(path[path.Length - 1]);
        //}

        //// find AP cost
        //int costToMove = ceilToInt(Pathfinding.CalculateTotalCost(path.Slice()) / settings.RangePerAP);

        //CommonWrites.ModifyStatFix<ActionPoints>(accessor, context.InstigatorPawn, -costToMove);

        //accessor.SetOrAddComponent(context.InstigatorPawn, new Destination() { Value = destinationPosition });

        return true;
    }
}