using System;
using UnityEngine;
using UnityEngineX;

public class GizmoPhysicsWorld : MonoBehaviour
{
    /*
    private void OnDrawGizmos()
    {
        var simWorld = GameMonoBehaviourHelpers.GetSimulationWorld();
        var physicsWorldSystem = simWorld?.GetExistingSystem<BuildPhysicsWorldSystem>();
        if (physicsWorldSystem == null)
            return;

        Gizmos.color = Color.green;
        foreach (var body in physicsWorldSystem.PhysicsWorld.DynamicBodies)
        {
            DrawBody(body);
        }

        Gizmos.color = Color.white;
        foreach (var body in physicsWorldSystem.PhysicsWorld.StaticBodies)
        {
            DrawBody(body);
        }
    }

    private void DrawBody(PhysicsBody physicsBody)
    {
        var aabb = physicsBody.Aabb;
        Gizmos.DrawWireCube((Vector2)aabb.Center, (Vector2)aabb.Extents);
    }
    */
}