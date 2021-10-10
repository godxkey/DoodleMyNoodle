using CCC.Fix2D;
using Unity.Collections;
using Unity.Entities;
using static fixMath;

[UpdateAfter(typeof(CreateAIPathToDestinationSystem))]
[UpdateInGroup(typeof(AISystemGroup))]
public class DirectAIAlongPathSystem : SimSystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        Entities
            .ForEach((DynamicBuffer<AIPathPosition> pathPositions, ref AIDestination destination, ref AIMoveInputLastFrame moveInputLastFrame, in ControlledEntity pawn) =>
            {
                if (pathPositions.IsEmpty && !moveInputLastFrame.WasAttemptingToMove)
                {
                    return;
                }

                if (!HasComponent<MoveInput>(pawn))
                    return;

                fix2 moveInput = new fix2(0, 0);

                if (pathPositions.Length > 0)
                {
                    var moveSpeed = GetComponent<MoveSpeed>(pawn);

                    if (moveSpeed.Value > 0)
                    {
                        fix2 pawnPos = GetComponent<FixTranslation>(pawn);
                        fix moveDist = moveSpeed.Value * deltaTime;
                        fix2 a = pawnPos;
                        fix2 b = pawnPos;
                        fix2 v = new fix2(0);
                        fix vLength = 0;
                        bool canRemovePositions = false;

                        while (moveDist > vLength && pathPositions.Length > 0)
                        {
                            moveDist -= vLength;
                            a = b;

                            if (canRemovePositions)
                            {
                                pathPositions.RemoveAt(0);
                            }
                            canRemovePositions = true;

                            if (pathPositions.Length > 0)
                            {
                                b = pathPositions[0].Value;
                            }

                            v = b - a;
                            vLength = length(v);
                        }

                        fix2 dir = (vLength == 0 ? fix2(0) : (v / vLength));
                        fix2 targetPosition = a + (dir * min(moveDist, vLength));
                        fix2 targetVelocity = (targetPosition - pawnPos) / deltaTime;

                        moveInput = targetVelocity / moveSpeed;
                    }
                }

                SetComponent<MoveInput>(pawn, moveInput);

                moveInputLastFrame.WasAttemptingToMove = moveInput != fix2(0, 0);
            }).Schedule();
    }
}
