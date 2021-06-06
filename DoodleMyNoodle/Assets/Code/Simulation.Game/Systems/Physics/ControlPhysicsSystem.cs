using CCC.Fix2D;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;
using static fixMath;
using static Unity.Mathematics.math;


[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateBefore(typeof(PhysicsWorldSystem))]
[AlwaysUpdateSystem]
public class ControlPhysicsSystem : SimSystemBase
{
    protected override void OnUpdate()
    {
        // Update the physics world settings
        if (!HasSingleton<PhysicsStepSettings>())
        {
            CreateSingleton(PhysicsStepSettings.Default);
        }

        var settings = GetSingleton<PhysicsStepSettings>();
        settings.TimeStep = (float)Time.DeltaTime;
        settings.GravityFix = SimulationGameConstants.Gravity;
        SetSingleton(settings);

        Entity player = Entity.Null;

        Entities.WithAll<PlayerTag>()
            .ForEach((Entity entity, in Active active) =>
            {
                if (active)
                {
                    player = entity;
                }
            }).Run();

        if (player != Entity.Null)
        {
            Entity pawn = GetComponent<ControlledEntity>(player);
            if (pawn != Entity.Null)
            {
                var collider = GetComponent<PhysicsColliderBlob>(pawn);
                unsafe
                {
                    //Log.Info("collider address: " + ((ulong)collider.Collider.GetUnsafePtr()).ToString());
                }
            }
        }
    }
}