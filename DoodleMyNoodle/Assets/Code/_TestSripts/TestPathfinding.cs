using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using CCC.Fix2D;

public class TestPathfinding : MonoBehaviour
{
    public Color GizmoColorWalk = new Color(1, 0.1f, 1, 1);
    public Color GizmoColorDrop = new Color(1, 0.1f, 0.1f, 1);
    public Color GizmoColorJump = new Color(0.1f, 0.1f, 1, 1);
    public Transform Start;
    public Transform Goal;
    public fix MaxSearchCost = 20;
    public fix Jump1TileCost = (fix)1.5;
    public fix Drop1TileCost = 1;
    public fix Walk1TileCost = 1;

    List<Pathfinding.PathResult> _pathsToDraw = new List<Pathfinding.PathResult>();

    private void OnEnable()
    {
        Start = new GameObject("TestPathfinding.Start").transform;
        Goal = new GameObject("TestPathfinding.End").transform;

        Start.SetParent(transform);
        Goal.SetParent(transform);
    }

    private void OnDisable()
    {
        Destroy(Start.gameObject);
        Destroy(Goal.gameObject);

        foreach (var item in _pathsToDraw)
        {
            item.Dispose();
        }
        _pathsToDraw.Clear();
    }

    private void Update()
    {
        var simWorld = PresentationHelpers.GetSimulationWorld();
        if (simWorld == null)
            return;

        foreach (var item in _pathsToDraw)
        {
            item.Dispose();
        }
        _pathsToDraw.Clear();

        var pathfindingContext = new Pathfinding.Context(CommonReads.GetTileWorld(simWorld), simWorld.GetExistingSystem<PhysicsWorldSystem>().PhysicsWorld, MaxSearchCost);
        pathfindingContext.AgentCapabilities.Jump1TileCost = Jump1TileCost;
        pathfindingContext.AgentCapabilities.Drop1TileCost = Drop1TileCost;
        pathfindingContext.AgentCapabilities.Walk1TileCost = Walk1TileCost;
        Pathfinding.PathResult result = new Pathfinding.PathResult(Allocator.Persistent);
        Pathfinding.FindNavigablePath(pathfindingContext, (fix2)(Vector2)Start.position, (fix2)(Vector2)Goal.position, reachDistance: 0, ref result);
        _pathsToDraw.Add(result);
    }

    private void OnDrawGizmos()
    {

        foreach (var path in _pathsToDraw)
        {
            fix2 a = path.StartingPosition;
            fix2 b;

            for (int i = 0; i < path.Segments.Length; i++)
            {
                b = path.Segments[i].EndPosition;

                switch (path.Segments[i].TransportToReach)
                {
                    case Pathfinding.TransportMode.Walk:
                        Gizmos.color = GizmoColorWalk;
                        break;
                    case Pathfinding.TransportMode.Drop:
                        Gizmos.color = GizmoColorDrop;
                        break;
                    case Pathfinding.TransportMode.Jump:
                        Gizmos.color = GizmoColorJump;
                        break;
                }

                Gizmos.DrawLine((Vector2)a, (Vector2)b);

                a = b;
            }
        }
    }
}