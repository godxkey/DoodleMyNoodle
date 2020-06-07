using static fixMath;
using System;
using UnityEngine;
using Unity.MathematicsX;
using Unity.Mathematics;

public class GizmoGridRect : MonoBehaviour
{
    public Color Color;

    private void OnDrawGizmos()
    {
        ExternalSimWorldAccessor simWorld = GameMonoBehaviourHelpers.GetSimulationWorld();

        if (simWorld == null && !simWorld.HasSingleton<GridInfo>())
            return;

        intRect gridRect = simWorld.GetSingleton<GridInfo>().GridRect;

        Gizmos.color = Color;

        Vector2 bottomLeft = new Vector2(gridRect.min.x, gridRect.min.y);
        Vector2 bottomRight = new Vector2(gridRect.max.x, gridRect.min.y);
        Vector2 TopLeft = new Vector2(gridRect.min.x, gridRect.max.y);
        Vector2 TopRight = new Vector2(gridRect.max.x, gridRect.max.y);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomLeft, TopLeft);
        Gizmos.DrawLine(TopLeft, TopRight);
        Gizmos.DrawLine(TopRight, bottomRight);
    }
}
