using Unity.Entities;
using CCC.Fix2D;
using Unity.Mathematics;
using Unity.Collections;

public class UpdateCanMoveSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        if (!HasSingleton<GameStartedTag>())
        {
            Entities
                .ForEach((ref CanMove canMove) =>
                {
                    canMove = false;
                }).Schedule();
            return;
        }

        Entities
            .ForEach((ref CanMove canMove, in DistanceFromTarget distanceFromTarget, in StopMoveFromTargetDistance stopMoveFromTargetDistance, in Health hp, in Grounded grounded) =>
            {
                canMove = distanceFromTarget.Value > stopMoveFromTargetDistance.Value

                    // entities need to be alive to move
                    && hp.Value > 0

                    // entities need to be grounded (nb: flying entities do not have the component)
                    && grounded;
            }).Schedule();

        Entities
            .WithNone<Grounded>()
            .ForEach((ref CanMove canMove, in DistanceFromTarget distanceFromTarget, in StopMoveFromTargetDistance stopMoveFromTargetDistance, in Health hp) =>
            {
                canMove = distanceFromTarget.Value > stopMoveFromTargetDistance.Value

                    // entities need to be alive to move
                    && hp.Value > 0;
            }).Schedule();
    }
}