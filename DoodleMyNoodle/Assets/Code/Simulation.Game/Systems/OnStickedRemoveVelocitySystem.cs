using CCC.Fix2D;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class OnStickedRemoveVelocitySystem : SimSystemBase
{
    protected override void OnUpdate()
    {
        fix2 gravity = fix2.zero;
        if (HasSingleton<PhysicsStepSettings>())
        {
            gravity = GetSingleton<PhysicsStepSettings>().GravityFix;
        }

        fix2 counterGravity = -gravity * Time.DeltaTime;

        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((Entity entityA, ref PhysicsVelocity physicsVelocity , in StickOnCollisionTag stickOnCollisionTag, in PhysicsGravity gravScale) =>
        {
            if (stickOnCollisionTag.Sticked)
            {
                physicsVelocity.Linear += counterGravity * gravScale.ScaleFix;
            }
        }).Run();
    }
}