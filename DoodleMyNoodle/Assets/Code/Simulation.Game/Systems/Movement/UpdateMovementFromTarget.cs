using Unity.Entities;
using CCC.Fix2D;

[UpdateInGroup(typeof(MovementSystemGroup))]
[UpdateAfter(typeof(UpdateTargetRelativePositionSystem))]
[UpdateBefore(typeof(UpdateMovementSystem))]
public class UpdateMovementFromTarget : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref OffsetFromTarget offsetFromTarget, in RemainingPeriodicActionCount remainingPeriodicActionCount, in KeepWalkingAfterPeriodicAction keepWalkingAfterPeriodicAction) =>
        {
            if (remainingPeriodicActionCount.Value == 0)
            {
                offsetFromTarget.Value = 1000; // big number
            }
        }).Schedule();
    }
}