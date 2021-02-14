using CCC.Fix2D;
using Unity.Entities;
using Unity.Mathematics;
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
        SetSingleton(settings);
    }
}