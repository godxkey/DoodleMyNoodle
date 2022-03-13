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
            .ForEach((ref CanMove canMove, in DistanceFromTarget distanceFromTarget, in StopMoveFromTargetDistance stopMoveFromTargetDistance, in Health hp) =>
            {
                canMove = distanceFromTarget.Value > stopMoveFromTargetDistance.Value

                    // entities need to be alive to move
                    && hp.Value > 0;
            }).Schedule();
    }
}