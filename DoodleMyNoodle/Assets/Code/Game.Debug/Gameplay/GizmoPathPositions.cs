using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using CCC.Fix2D;

public class GizmoPathPositions : MonoBehaviour
{
    [SerializeField] private Color _color;
    [SerializeField] private Color _colorJump;
    [SerializeField] private Color _colorDrop;

    List<(Vector3 a, Vector3 b, Color color)> _lines = new List<(Vector3, Vector3, Color)>();

    private void OnDrawGizmos()
    {
        var simWorld = PresentationHelpers.GetSimulationWorld();

        if (simWorld == null)
            return;

        _lines.Clear();

        simWorld.Entities
            .WithAll<PathPosition>()
            .ForEach((Entity entity, ref FixTranslation translation) =>
            {
                DynamicBuffer<PathPosition> path = simWorld.GetBufferReadOnly<PathPosition>(entity);

                Vector3 p1 = translation.Value.ToUnityVec();

                for (int i = 0; i < path.Length; i++)
                {
                    Vector3 p2 = path[i].Position.ToUnityVec();
                    _lines.Add((p1, p2, _color));
                    p1 = p2;
                }
            });

        simWorld.Entities
            .WithAll<AIPathSegment>()
            .ForEach((Entity entity, ref ControlledEntity pawn) =>
            {
                if (!simWorld.HasComponent<FixTranslation>(pawn))
                    return;

                var translation = simWorld.GetComponent<FixTranslation>(pawn);

                DynamicBuffer<AIPathSegment> path = simWorld.GetBufferReadOnly<AIPathSegment>(entity);

                Vector3 p1 = translation.Value.ToUnityVec();

                for (int i = 0; i < path.Length; i++)
                {
                    Vector3 p2 = path[i].Value.EndPosition.ToUnityVec();

                    Color color = _color;
                    switch (path[i].Value.TransportToReach)
                    {
                        case Pathfinding.TransportMode.Drop:
                            color = _colorDrop;
                            break;
                        case Pathfinding.TransportMode.Jump:
                            color = _colorJump;
                            break;
                    }
                    
                    _lines.Add((p1, p2, color));
                    p1 = p2;
                }
            });



        foreach (var line in _lines)
        {
            Gizmos.color = line.color;
            Gizmos.DrawLine(line.a, line.b);
        }
    }
}
