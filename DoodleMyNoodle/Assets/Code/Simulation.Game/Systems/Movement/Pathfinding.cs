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
    public const int MAX_PATH_LENGTH = 20;

    public static fix CalculateTotalCost(NativeSlice<int2> path)
    {
        // for now, length = cost
        return CalculateTotalLength(path);
    }

    public static fix CalculateTotalLength(NativeSlice<int2> path)
    {
        return path.Length - 1; // first node is always the 'starting' point
    }

    public static int GetLastPathPointReachableWithinCost(NativeSlice<int2> path, fix maxCost)
    {
        if (maxCost <= 0)
            return 0;

        if (maxCost > CalculateTotalCost(path))
        {
            return path.Length - 1;
        }

        return (int)floor(maxCost);
    }

    public struct TileCostPair
    {
        public int2 Tile;
        public fix ReachCost;

        public TileCostPair(int2 tile, fix reachCost)
        {
            Tile = tile;
            ReachCost = reachCost;
        }
    }

    public static bool NavigableFloodSearch<T>(TileWorld tileWorld, int2 start, fix maxCost, NativeQueue<TileCostPair> buffer, ref T stopPredicate, out int2 result) where T : ITilePredicate
    {
        NativeHashSet<int2> visited = new NativeHashSet<int2>(40, Allocator.Temp);
        NativeQueue<TileCostPair> toVisit = buffer;
        toVisit.Clear();
        toVisit.Enqueue(new TileCostPair(start, 0));

        while (!toVisit.IsEmpty())
        {
            TileCostPair x = toVisit.Dequeue();
            if (x.ReachCost <= maxCost && tileWorld.CanStandOn(x.Tile))
            {
                if (stopPredicate.Evaluate(x.Tile))
                {
                    result = x.Tile;
                    return true;
                }

                // add neighbords
                add_neighbor(int2(1, 0));
                add_neighbor(int2(0, 1));
                add_neighbor(int2(-1, 0));
                add_neighbor(int2(0, -1));

                void add_neighbor(int2 dir)
                {
                    int2 neighbor = x.Tile + dir;

                    if (!visited.Contains(neighbor))
                    {
                        fix neighborCost = x.ReachCost + d(x.Tile, neighbor);
                        toVisit.Enqueue(new TileCostPair(neighbor, neighborCost));
                    }
                }
            }

            visited.Add(x.Tile);
        }

        result = default;
        return false;
    }

    public static bool FindCheapestNavigablePath(TileWorld tileWorld, int2 start, NativeSlice<int2> goals, fix maxLength, NativeList<int2> result)
    {
        Profiler.BeginSample("Pathfinding");
        result.Clear();

        if (goals.Length == 0)
        {
            Log.Error($"'goals' argument is empty.");
            return false;
        }

        if (maxLength > MAX_PATH_LENGTH)
        {
            Debug.LogWarning($"Path Finding Max Cost cannot exceed {MAX_PATH_LENGTH}");
            maxLength = MAX_PATH_LENGTH;
        }

        for (int i = 0; i < goals.Length; i++)
        {
            if (goals[i].Equals(start))
            {
                result.Add(goals[i]);
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

        NativeList<int2> resultInvertedPath = new NativeList<int2>(Allocator.Temp);
        NativeList<int2> neighbors = new NativeList<int2>(Allocator.Temp);

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
        NativeHashMap<int2, int2> cameFrom = new NativeHashMap<int2, int2>(100, Allocator.Temp);

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

        while (openSet.Length != 0)
        {
            // This operation could occur in O(1) time if openSet was a min-heap or a priority queue
            int2 current = node_in_openSet_having_the_lowest_gScore_value();

            for (int i = 0; i < goals.Length; i++)
            {
                if (current.Equals(goals[i]))
                {
                    reconstruct_path(cameFrom, current, resultInvertedPath);
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

            get_neighbors_standOn(current, neighbors, tileWorld);
            for (int i = 0; i < neighbors.Length; i++)
            {
                // d(current,neighbor) is the weight of the edge from current to neighbor
                // neighbor_gScore is the distance from start to the neighbor through current
                fix neighbor_gScore = gScore[current] + d(current, neighbors[i]);
                if (neighbor_gScore <= maxLength && (!gScore.TryGetValue(neighbors[i], out fix s) || neighbor_gScore < s))
                {
                    // This path to neighbor is better than any previous one. Record it!

                    cameFrom.SetOrAdd(neighbors[i], current);
                    gScore.SetOrAdd(neighbors[i], neighbor_gScore);
                    if (!closeSet.Contains(neighbors[i]))
                        openSet.Add(neighbors[i]);
                }
            }
        }

        construct_result(resultInvertedPath, result);

        resultInvertedPath.Dispose();
        neighbors.Dispose();
        openSet.Dispose();
        closeSet.Dispose();
        cameFrom.Dispose();
        gScore.Dispose();

        Profiler.EndSample();
        return pathFound;


        // local functions
        NativeSlice<int2> sanitize_goals(NativeSlice<int2> currentGoals)
        {
            NativeArray<int2> newGoals = default;
            int newGoalsCount = currentGoals.Length;
            for (int i = currentGoals.Length - 1; i >= 0; i--)
            {
                if (lengthmanhattan(start - currentGoals[i]) > maxLength || !tileWorld.CanStandOn(currentGoals[i]))
                {
                    if (!newGoals.IsCreated)
                    {
                        newGoals = new NativeArray<int2>(currentGoals.Length, Allocator.Temp);
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

    public static bool FindNavigablePath(ISimWorldReadAccessor accessor, int2 start, int2 goal, fix maxLength, NativeList<int2> result)
    {
        return FindNavigablePath(CommonReads.GetTileWorld(accessor), start, goal, maxLength, result);
    }

    public static bool FindNavigablePath(TileWorld tileWorld, int2 start, int2 goal, fix maxLength, NativeList<int2> result)
    {
        result.Clear();

        if (maxLength > MAX_PATH_LENGTH)
        {
            Debug.LogWarning($"Path Finding Max Cost cannot exceed {MAX_PATH_LENGTH}");
            maxLength = MAX_PATH_LENGTH;
        }

        if (start.Equals(goal))
        {
            result.Add(goal);
            return true;
        }

        // Destination cannot be reached
        if (lengthmanhattan(start - goal) > maxLength || !tileWorld.CanStandOn(goal))
        {
            return false;
        }

        NativeList<int2> resultInvertedPath = new NativeList<int2>(Allocator.Temp);
        NativeList<int2> neighbors = new NativeList<int2>(Allocator.Temp);

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
        NativeHashMap<int2, int2> cameFrom = new NativeHashMap<int2, int2>(100, Allocator.Temp);

        // For node n, gScore[n] is the cost of the cheapest path from start to n currently known.
        NativeHashMap<int2, fix> gScore = new NativeHashMap<int2, fix>(100, Allocator.Temp);
        // default value is Infinity
        gScore[start] = 0;

        // For node n, fScore[n] := gScore[n] + h(n). fScore[n] represents our current best guess as to
        // how short a path from start to finish can be if it goes through n.
        NativeHashMap<int2, fix> fScore = new NativeHashMap<int2, fix>(100, Allocator.Temp);
        // default value is Infinity
        fScore[start] = h(start, goal);


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

        bool pathFound = false;
        while (openSet.Length != 0)
        {
            // This operation could occur in O(1) time if openSet was a min-heap or a priority queue
            int2 current = node_in_openSet_having_the_lowest_fScore_value();
            if (current.Equals(goal))
            {
                reconstruct_path(cameFrom, current, resultInvertedPath);
                pathFound = true;
                break;
            }

            // Current node goes into the closed set
            closeSet.Add(current);

            openSet.RemoveAtSwapBack(openSet.IndexOf(current));

            get_neighbors_standOn(current, neighbors, tileWorld);
            for (int i = 0; i < neighbors.Length; i++)
            {
                // d(current,neighbor) is the weight of the edge from current to neighbor
                // neighbor_gScore is the distance from start to the neighbor through current
                fix neighbor_gScore = gScore[current] + d(current, neighbors[i]);
                if (neighbor_gScore <= maxLength && (!gScore.TryGetValue(neighbors[i], out fix s) || neighbor_gScore < s))
                {
                    // This path to neighbor is better than any previous one. Record it!

                    cameFrom.SetOrAdd(neighbors[i], current);
                    gScore.SetOrAdd(neighbors[i], neighbor_gScore);
                    fScore.SetOrAdd(neighbors[i], neighbor_gScore + h(neighbors[i], goal));
                    if (!closeSet.Contains(neighbors[i]))
                        openSet.Add(neighbors[i]);
                }
            }
        }

        construct_result(resultInvertedPath, result);

        resultInvertedPath.Dispose();
        neighbors.Dispose();
        openSet.Dispose();
        closeSet.Dispose();
        cameFrom.Dispose();
        gScore.Dispose();
        fScore.Dispose();
        return pathFound;
    }

    private static void construct_result(NativeList<int2> invertedPath, NativeList<int2> result)
    {
        if (result.Capacity < invertedPath.Length)
            result.Capacity = invertedPath.Capacity;

        for (int i = invertedPath.Length - 1; i >= 0; i--)
        {
            result.Add(invertedPath[i]);
        }
    }

    private static void reconstruct_path(NativeHashMap<int2, int2> cameFrom, int2 current, NativeList<int2> result)
    {
        result.Add(current);
        while (cameFrom.TryGetValue(current, out int2 cameFromValue))
        {
            current = cameFromValue;
            result.Add(current);
        }
    }

    // d(current,neighbor) is the weight of the edge from current to neighbor
    private static fix d(int2 current, int2 neighbor)
    {
        return 1; // in a 2D grid, neighbor weight is always 1
    }

    private static readonly int2[] s_directionVectors = new int2[]
    {
        int2(-1, 0), // left
        int2(1, 0), // right
        int2(0, 1), // up
        int2(0, -1) // down
    };

    private static void get_neighbors_standOn(int2 tile, NativeList<int2> neighbors, TileWorld tileWorld)
    {
        neighbors.Clear();

        checkNeighbor(int2(0, 1));
        checkNeighbor(int2(1, 0));
        checkNeighbor(int2(0, -1));
        checkNeighbor(int2(-1, 0));

        void checkNeighbor(int2 dir)
        {
            int2 neighborPos = tile + dir;

            if (tileWorld.CanStandOn(neighborPos))
            {
                neighbors.Add(neighborPos);
            }
        }
    }

    // h is the heuristic function. h(n) estimates the cost to reach goal from node n.
    private static fix h(int2 tile, int2 goal)
    {
        int2 delta = abs(goal - tile);
        //delta *= delta; // making tiles in diagonal more appealing than tiles in a straight line far away
        return delta.x + delta.y;
    }
}
