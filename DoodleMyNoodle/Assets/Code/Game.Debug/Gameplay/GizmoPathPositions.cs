using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using CCC.Fix2D;

public class GizmoPathPositions : MonoBehaviour
{
    [SerializeField] private Color _color;

    List<(Vector3 a, Vector3 b)> _lines = new List<(Vector3 a, Vector3 b)>();

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
                    _lines.Add((p1, p2));
                    p1 = p2;
                }
            });


        Gizmos.color = _color;

        foreach (var line in _lines)
        {
            Gizmos.DrawLine(line.a, line.b);
        }
    }
}
