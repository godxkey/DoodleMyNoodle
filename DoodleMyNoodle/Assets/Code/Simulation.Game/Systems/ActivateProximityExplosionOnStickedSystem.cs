using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class ActivateProximityExplosionOnStickedSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        Entities
        .WithoutBurst()
        .WithStructuralChanges()
        .ForEach((Entity entityA, ref ExplodeOnProximity explodeOnProximity, in StickOnCollisionTag stickOnCollisionTag) =>
        {
            if (stickOnCollisionTag.Sticked)
            {
                explodeOnProximity.Activated = true;
            }
        }).Run();
    }
}