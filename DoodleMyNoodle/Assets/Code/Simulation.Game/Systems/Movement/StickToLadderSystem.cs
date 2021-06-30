using CCC.Fix2D;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

// [UpdateBefore(typeof(PhysicsSystemGroup))]  // implicit
[UpdateBefore(typeof(DirectAlongPathSystem))]
[UpdateInGroup(typeof(MovementSystemGroup))]
public class StickToLadderSystem : SimSystemBase
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
            .ForEach((ref PhysicsVelocity velocity, in NavAgentFootingState footing, in PhysicsGravity gravScale) =>
        {
            if (footing.Value == NavAgentFooting.Ladder)
            {
                velocity.Linear += counterGravity * gravScale.ScaleFix;// movetowards(velocity.Linear, fix2.zero, 2);
            }

        }).Run();
    }
}