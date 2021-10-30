using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngineX;
using static fixMath;
using static Unity.Mathematics.math;
using static Unity.MathematicsX.mathX;

public static class Pathfinding
{
    public struct PathResult : IDisposable
    {
        public fix2 StartingPosition;
        public NativeList<Segment> Segments;
        public fix TotalCost;

        public PathResult(Allocator allocator)
        {
            Segments = new NativeList<Segment>(allocator);
            TotalCost = 0;
            StartingPosition = default;
        }

        public void Dispose()
        {
            Segments.Dispose();
        }

        public int LastReachableSegmentWithinCost(fix maxCost)
        {
            if (Segments.Length == 0)
                return -1;

            if (maxCost < TotalCost)
                return Segments.Length - 1;

            fix cost = 0;
            int i = 0;

            for (; i < Segments.Length - 1; i++)
            {
                cost += Segments[i].CostToReach;

                if (cost > maxCost)
                    break;
            }
            return i;
        }

        internal void Reset()
        {
            StartingPosition = fix2.zero;
            Segments.Clear();
            TotalCost = 0;
        }
    }

    public enum TransportMode
    {
        Walk,
        Drop,
        Jump,
    }

    public struct Segment
    {
        public fix2 EndPosition;
        public int2 EndTile => Helpers.GetTile(EndPosition);
        public fix CostToReach;
        public TransportMode TransportToReach;

        public Segment(fix2 position) : this()
        {
            TransportToReach = TransportMode.Walk;
            EndPosition = position;
        }

        public Segment(fix2 position, TransportMode transportMode, fix cost)
        {
            EndPosition = position;
            TransportToReach = transportMode;
            CostToReach = cost;
        }
    }

    private struct SegmentInternal
    {
        public int2 StartTile;
        public TransportMode TransportToNext;
        public fix CostToNext;
    }

    public struct Context
    {
        public TileWorld TileWorld;
        public AgentCapabilities AgentCapabilities;

        public Context(TileWorld tileWorld)
        {
            AgentCapabilities = AgentCapabilities.Default;
            TileWorld = tileWorld;
        }
    }

    public struct AgentCapabilities
    {
        public fix Jump1TileCost;
        public fix Drop1TileCost;
        public fix Walk1TileCost;

        public static fix DefaultMaxCost => 20;

        public static AgentCapabilities Default => new AgentCapabilities()
        {
            Jump1TileCost = 1,
            Walk1TileCost = 1,
            Drop1TileCost = 0,
        };
    }

    public struct TileInfo
    {
        public int2 Position;
        public TileFlagComponent Flags;

        public TileInfo(int2 position, ref TileWorld tileWorld)
        {
            Position = position;
            Flags = tileWorld.GetFlags(position);
        }
    }

    private const int MAX_ITERATION_COUNT = 200;

    public static fix CalculateTotalCost(Context context, NativeSlice<int2> path)
    {
        if (path.Length == 0)
            return 0;

        fix cost = 0;

        TileInfo tileInfoA = new TileInfo(path[0], ref context.TileWorld);
        TileInfo tileInfoB;

        for (int i = 0; i < path.Length - 1; i++)
        {
            tileInfoB = new TileInfo(path[i + 1], ref context.TileWorld);
            cost += d(ref context, tileInfoA, tileInfoB, out _);
            tileInfoA = tileInfoB;
        }

        return cost;
    }

    public struct TileCostPair
    {
        public TileInfo Tile;
        public fix ReachCost;

        public TileCostPair(TileInfo tile, fix reachCost)
        {
            Tile = tile;
            ReachCost = reachCost;
        }
    }

    public static bool NavigableFloodSearch<T>(Context context, int2 start, fix maxCost, NativeQueue<TileCostPair> buffer, ref T stopPredicate, out int2 result) where T : ITilePredicate
    {
        NativeHashSet<int2> visited = new NativeHashSet<int2>(40, Allocator.Temp);
        NativeQueue<TileCostPair> toVisit = buffer;
        NativeList<TileInfo> neighbors = new NativeList<TileInfo>(Allocator.Temp);
        toVisit.Clear();
        toVisit.Enqueue(new TileCostPair(new TileInfo(start, ref context.TileWorld), 0));

        const int NEIGHBOR_DIRECTIONS_COUNT = 8;
        unsafe
        {
            int2* neighborDirections = stackalloc int2[NEIGHBOR_DIRECTIONS_COUNT]
            {
                int2(0, 1),
                int2(0, -1),
                int2(1, 0),
                int2(-1, 0),
                int2(1, 1),
                int2(-1, -1),
                int2(1, -1),
                int2(-1, 1),
            };

            int iterations = 0;
            while (!toVisit.IsEmpty())
            {
                TileCostPair x = toVisit.Dequeue();
                if (x.ReachCost <= maxCost)
                {
                    if (stopPredicate.Evaluate(x.Tile.Position))
                    {
                        result = x.Tile.Position;
                        return true;
                    }

                    get_neighbors(x.Tile.Position, neighbors, ref context.TileWorld);

                    for (int i = 0; i < neighbors.Length; i++)
                    {
                        if (!visited.Contains(neighbors[i].Position))
                        {
                            fix neighborCost = x.ReachCost + d(ref context, x.Tile, neighbors[i], out _);
                            toVisit.Enqueue(new TileCostPair(neighbors[i], neighborCost));
                        }
                    }
                }

                visited.Add(x.Tile.Position);

                if (IncrementAndCheckIterationBreak(ref iterations))
                    break;
            }
        }
        result = default;
        return false;
    }

    public static bool FindCheapestNavigablePath(Context context, fix2 startPos, NativeSlice<fix2> goals, fix maxCost, ref PathResult result)
    {
        Profiler.BeginSample("Pathfinding");
        result.Reset();
        result.StartingPosition = startPos;

        int2 start = Helpers.GetTile(startPos);

        if (goals.Length == 0)
        {
            Log.Error($"'goals' argument is empty.");
            return false;
        }

        for (int i = 0; i < goals.Length; i++)
        {
            if (goals[i].Equals(start))
            {
                result.Segments.Add(new Segment(Helpers.GetTileCenter(goals[i])));
                Profiler.EndSample();
                return true;
            }
        }

        goals = sanitize_goals(goals);

        // Destination cannot be reached
        if (goals.Length == 0)
        {
            Profiler.EndSample();
            return false;
        }

        NativeList<TileInfo> neighbors = new NativeList<TileInfo>(Allocator.Temp);

        // The set of discovered nodes that may need to be (re-)expanded.
        // Initially, only the start node is known.
        // This is usually implemented as a min-heap or priority queue rather than a hash-set.
        NativeList<int2> openSet = new NativeList<int2>(Allocator.Temp)
        {
            start
        };

        // List of nodes already discovered and explored. 
        // Starts off empty
        // Once a node has been 'current' it then goes here
        NativeList<int2> closeSet = new NativeList<int2>(Allocator.Temp);

        // For node n, cameFrom[n] is the node immediately preceding it on the cheapest path from start
        // to n currently known.
        NativeHashMap<int2, SegmentInternal> cameFrom = new NativeHashMap<int2, SegmentInternal>(100, Allocator.Temp);

        // For node n, gScore[n] is the cost of the cheapest path from start to n currently known.
        NativeHashMap<int2, fix> gScore = new NativeHashMap<int2, fix>(100, Allocator.Temp);
        // default value is Infinity
        gScore[start] = 0;


        int2 node_in_openSet_having_the_lowest_gScore_value()
        {
            int2 lowest = default;
            fix lowestValue = fix.MaxValue;
            for (int i = 0; i < openSet.Length; i++)
            {
                if (gScore.TryGetValue(openSet[i], out fix value) && value < lowestValue)
                {
                    lowest = openSet[i];
                    lowestValue = value;
                }
            }
            return lowest;
        }

        bool pathFound = false;
        int iterations = 0;
        while (openSet.Length != 0)
        {
            // This operation could occur in O(1) time if openSet was a min-heap or a priority queue
            int2 current = node_in_openSet_having_the_lowest_gScore_value();

            for (int i = 0; i < goals.Length; i++)
            {
                if (current.Equals(Helpers.GetTile(goals[i])))
                {
                    result.TotalCost = gScore[current];
                    construct_result_path(ref context, cameFrom, current, startPos, ref result);
                    pathFound = true;
                    break;
                }
            }

            if (pathFound)
            {
                break;
            }

            // Current node goes into the closed set
            closeSet.Add(current);

            openSet.RemoveAtSwapBack(openSet.IndexOf(current));

            get_neighbors(current, neighbors, ref context.TileWorld);
            for (int i = 0; i < neighbors.Length; i++)
            {
                // d(current,neighbor) is the weight of the edge from current to neighbor
                // neighbor_gScore is the distance from start to the neighbor through current
                fix cost_to_neighbor = d(ref context, new TileInfo(current, ref context.TileWorld), neighbors[i], out TransportMode transportMode);
                fix neighbor_gScore = gScore[current] + cost_to_neighbor;
                if (neighbor_gScore <= maxCost && (!gScore.TryGetValue(neighbors[i].Position, out fix s) || neighbor_gScore < s))
                {
                    // This path to neighbor is better than any previous one. Record it!
                    SegmentInternal segment = new SegmentInternal() { StartTile = current, TransportToNext = transportMode, CostToNext = cost_to_neighbor };
                    cameFrom.SetOrAdd(neighbors[i].Position, segment);
                    gScore.SetOrAdd(neighbors[i].Position, neighbor_gScore);
                    if (!closeSet.Contains(neighbors[i].Position))
                        openSet.Add(neighbors[i].Position);
                }
            }

            if (IncrementAndCheckIterationBreak(ref iterations))
                break;
        }

        neighbors.Dispose();
        openSet.Dispose();
        closeSet.Dispose();
        cameFrom.Dispose();
        gScore.Dispose();

        Profiler.EndSample();
        return pathFound;


        // local functions
        NativeSlice<fix2> sanitize_goals(NativeSlice<fix2> currentGoals)
        {
            NativeArray<fix2> newGoals = default;
            int newGoalsCount = currentGoals.Length;
            for (int i = currentGoals.Length - 1; i >= 0; i--)
            {
                if (lengthmanhattan(startPos - currentGoals[i]) > maxCost || !context.TileWorld.CanStandOn(Helpers.GetTile(currentGoals[i])))
                {
                    if (!newGoals.IsCreated)
                    {
                        newGoals = new NativeArray<fix2>(currentGoals.Length, Allocator.Temp);
                        currentGoals.CopyTo(newGoals);
                    }

                    // remove swap back
                    newGoals[i] = newGoals[newGoalsCount - 1];
                    newGoalsCount--;
                }
            }

            return newGoals.IsCreated ? newGoals.Slice(0, newGoalsCount) : currentGoals;
        }
    }

    private static bool IncrementAndCheckIterationBreak(ref int iterations)
    {
        iterations++;
        if (iterations > MAX_ITERATION_COUNT)
        {
            Debug.LogWarning($"Path not found because we searched for too long. Consider trying to look for a closer target. Or increase the {nameof(MAX_ITERATION_COUNT)} constant.");
            return true;
        }
        return false;
    }

    public static bool FindNavigablePath(Context context, fix2 startPos, fix2 goalPos, fix maxCost, ref PathResult result)
    {
        result.Reset();
        result.StartingPosition = startPos;

        int2 start = Helpers.GetTile(startPos);
        int2 goal = Helpers.GetTile(goalPos);

        if (start.Equals(goal))
        {
            result.Segments.Add(new Segment() { EndPosition = Helpers.GetTileCenter(goal), TransportToReach = TransportMode.Walk });
            return true;
        }

        // Destination cannot be reached
        if (h(start, goal, ref context.AgentCapabilities) > maxCost || !context.TileWorld.CanStandOn(goal))
        {
            return false;
        }

        NativeList<TileInfo> neighbors = new NativeList<TileInfo>(Allocator.Temp);

        // The set of discovered nodes that may need to be (re-)expanded.
        // Initially, only the start node is known.
        // This is usually implemented as a min-heap or priority queue rather than a hash-set.
        NativeList<int2> openSet = new NativeList<int2>(Allocator.Temp)
        {
            start
        };

        // List of nodes already discovered and explored. 
        // Starts off empty
        // Once a node has been 'current' it then goes here
        NativeList<int2> closeSet = new NativeList<int2>(Allocator.Temp);

        // For node n, cameFrom[n] is the node immediately preceding it on the cheapest path from start
        // to n currently known.
        NativeHashMap<int2, SegmentInternal> cameFrom = new NativeHashMap<int2, SegmentInternal>(100, Allocator.Temp);

        // For node n, gScore[n] is the cost of the cheapest path from start to n currently known.
        NativeHashMap<int2, fix> gScore = new NativeHashMap<int2, fix>(100, Allocator.Temp);
        // default value is Infinity
        gScore[start] = 0;

        // For node n, fScore[n] := gScore[n] + h(n). fScore[n] represents our current best guess as to
        // how short a path from start to finish can be if it goes through n.
        NativeHashMap<int2, fix> fScore = new NativeHashMap<int2, fix>(100, Allocator.Temp);
        // default value is Infinity
        fScore[start] = h(start, goal, ref context.AgentCapabilities);


        int2 node_in_openSet_having_the_lowest_fScore_value()
        {
            int2 lowest = default;
            fix lowestValue = fix.MaxValue;
            for (int i = 0; i < openSet.Length; i++)
            {
                if (fScore.TryGetValue(openSet[i], out fix value) && value < lowestValue)
                {
                    lowest = openSet[i];
                    lowestValue = value;
                }
            }
            return lowest;
        }

        int iterations = 0;
        bool pathFound = false;
        while (openSet.Length != 0)
        {
            // This operation could occur in O(1) time if openSet was a min-heap or a priority queue
            int2 current = node_in_openSet_having_the_lowest_fScore_value();
            if (current.Equals(goal))
            {
                result.TotalCost = gScore[current];
                construct_result_path(ref context, cameFrom, current, startPos, ref result);
                pathFound = true;
                break;
            }

            // Current node goes into the closed set
            closeSet.Add(current);

            openSet.RemoveAtSwapBack(openSet.IndexOf(current));

            get_neighbors(current, neighbors, ref context.TileWorld);
            for (int i = 0; i < neighbors.Length; i++)
            {
                // d(current,neighbor) is the weight of the edge from current to neighbor
                // neighbor_gScore is the distance from start to the neighbor through current
                fix cost_to_neighbor = d(ref context, new TileInfo(current, ref context.TileWorld), neighbors[i], out TransportMode transportMode);
                fix neighbor_gScore = gScore[current] + cost_to_neighbor;
                if (neighbor_gScore <= maxCost && (!gScore.TryGetValue(neighbors[i].Position, out fix s) || neighbor_gScore < s))
                {
                    // This path to neighbor is better than any previous one. Record it!
                    SegmentInternal segment = new SegmentInternal() { StartTile = current, TransportToNext = transportMode, CostToNext = cost_to_neighbor };
                    cameFrom.SetOrAdd(neighbors[i].Position, segment);
                    gScore.SetOrAdd(neighbors[i].Position, neighbor_gScore);
                    fix heuristic = h(neighbors[i].Position, goal, ref context.AgentCapabilities);
                    fScore.SetOrAdd(neighbors[i].Position, neighbor_gScore + heuristic);
                    if (!closeSet.Contains(neighbors[i].Position))
                        openSet.Add(neighbors[i].Position);
                }
            }

            if (IncrementAndCheckIterationBreak(ref iterations))
                break;
        }

        neighbors.Dispose();
        openSet.Dispose();
        closeSet.Dispose();
        cameFrom.Dispose();
        gScore.Dispose();
        fScore.Dispose();
        return pathFound;
    }

    private static void construct_result_path(ref Context context, NativeHashMap<int2, SegmentInternal> cameFrom, int2 currentTile, fix2 startPos, ref PathResult result)
    {
        while (cameFrom.TryGetValue(currentTile, out SegmentInternal cameFromSegment))
        {
            result.Segments.Add(new Segment() { EndPosition = Helpers.GetTileCenter(currentTile), TransportToReach = cameFromSegment.TransportToNext, CostToReach = cameFromSegment.CostToNext });
            currentTile = cameFromSegment.StartTile;
        }

        // small hack to make sure agents don't stick on walls when going up/down ladders
        {
            // insert a segment at the very start of the path that re-centers the agent on the ladder
            TileInfo startTile = new TileInfo(Helpers.GetTile(startPos), ref context.TileWorld);
            TileInfo firstTileToReach = new TileInfo(result.Segments.Last().EndTile, ref context.TileWorld);
            if (startTile.Flags.IsLadder && firstTileToReach.Flags.IsLadder && startTile.Position.x == firstTileToReach.Position.x && startTile.Position.y != firstTileToReach.Position.y)
            {
                result.Segments.Add(new Segment()
                {
                    CostToReach = context.AgentCapabilities.Walk1TileCost,
                    EndPosition = new fix2(Helpers.GetTileCenter(startPos).x, startPos.y),
                    TransportToReach = TransportMode.Walk,
                });
                
                result.TotalCost += context.AgentCapabilities.Walk1TileCost;
            }

        }

        result.Segments.Reverse();
    }

    private static void get_neighbors(int2 tile, NativeList<TileInfo> neighbors, ref TileWorld tileWorld)
    {
        neighbors.Clear();

        TileInfo current = new TileInfo(tile, ref tileWorld);
        TileInfo up = new TileInfo(tile + int2(0, 1), ref tileWorld);
        TileInfo down = new TileInfo(tile + int2(0, -1), ref tileWorld);
        TileInfo left = new TileInfo(tile + int2(-1, 0), ref tileWorld);
        TileInfo right = new TileInfo(tile + int2(1, 0), ref tileWorld);

        TileInfo topLeft = new TileInfo(tile + int2(-1, 1), ref tileWorld);
        TileInfo topRight = new TileInfo(tile + int2(1, 1), ref tileWorld);
        TileInfo bottomLeft = new TileInfo(tile + int2(-1, -1), ref tileWorld);
        TileInfo bottomRight = new TileInfo(tile + int2(1, -1), ref tileWorld);

        if (canStandOn(ref up, ref current))
            neighbors.Add(up);

        if (canStandOn(ref left, ref bottomLeft))
            neighbors.Add(left);

        if (canStandOn(ref right, ref bottomRight))
            neighbors.Add(right);

        if (canStandOn2(ref down, ref tileWorld))
            neighbors.Add(down);

        if (left.Flags.IsEmpty && canStandOn2(ref bottomLeft, ref tileWorld))
            neighbors.Add(bottomLeft);

        if (right.Flags.IsEmpty && canStandOn2(ref bottomRight, ref tileWorld))
            neighbors.Add(bottomRight);

        if (!up.Flags.IsTerrain)
        {
            if (canStandOn(ref topLeft, ref left))
                neighbors.Add(topLeft);

            if (canStandOn(ref topRight, ref right))
                neighbors.Add(topRight);
        }

        bool canStandOn2(ref TileInfo tile, ref TileWorld t)
        {
            TileInfo underTile = new TileInfo(tile.Position + int2(0, -1), ref t);
            return canStandOn(ref tile, ref underTile);
        }

        bool canStandOn(ref TileInfo tile, ref TileInfo underTile)
        {
            return tile.Flags.IsLadder || (tile.Flags.IsEmpty && underTile.Flags.IsTerrain);
        }
    }

    // d(current,neighbor) is the weight of the edge from current to neighbor
    private static fix d(ref Context context, TileInfo current, TileInfo neighbor, out TransportMode transportMode)
    {
        transportMode = TransportMode.Walk;
        fix cost = current.Position.x != neighbor.Position.x ? context.AgentCapabilities.Walk1TileCost : 0;

        if (current.Position.y < neighbor.Position.y)
        {
            if (current.Flags.IsLadder && neighbor.Flags.IsLadder)
            {
                cost += context.AgentCapabilities.Walk1TileCost;
                transportMode = TransportMode.Walk;
            }
            else
            {
                cost += context.AgentCapabilities.Jump1TileCost;
                transportMode = TransportMode.Jump;
            }
        }
        else if (current.Position.y > neighbor.Position.y)
        {
            if (current.Flags.IsLadder && neighbor.Flags.IsLadder)
            {
                cost += context.AgentCapabilities.Walk1TileCost;
                transportMode = TransportMode.Walk;
            }
            else
            {
                cost += context.AgentCapabilities.Drop1TileCost;
                transportMode = TransportMode.Drop;
            }
        }

        return cost;
    }

    // h is the heuristic function. h(n) estimates the cost to reach goal from node n. This method should be optimistic
    private static fix h(int2 tile, int2 goal, ref AgentCapabilities agentCapabilities)
    {
        return abs(tile.x - goal.x) * agentCapabilities.Walk1TileCost
            + max(goal.y - tile.y, 0) * min(agentCapabilities.Walk1TileCost, agentCapabilities.Jump1TileCost)
            + max(tile.y - goal.y, 0) * min(agentCapabilities.Walk1TileCost, agentCapabilities.Drop1TileCost);
    }
}
