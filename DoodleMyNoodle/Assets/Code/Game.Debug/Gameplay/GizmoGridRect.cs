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

        if (simWorld == null || !simWorld.HasSingleton<GridInfo>())
            return;

        GridInfo gridRect = simWorld.GetSingleton<GridInfo>();

        Rect visualRect = new Rect(new Vector2(gridRect.TileMin.x, gridRect.TileMin.y), new Vector2(gridRect.Width, gridRect.Height));

        // offset 0.5f pos
        visualRect.position -= Vector2.one * 0.5f;

        Vector2 bottomLeft = visualRect.min;
        Vector2 bottomRight = new Vector2(visualRect.max.x, visualRect.min.y);
        Vector2 TopLeft = new Vector2(visualRect.min.x, visualRect.max.y);
        Vector2 TopRight = visualRect.max;

        Gizmos.color = Color;
        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomLeft, TopLeft);
        Gizmos.DrawLine(TopLeft, TopRight);
        Gizmos.DrawLine(TopRight, bottomRight);
    }
}
