using Unity.Entities;
using static Unity.Mathematics.math;
using static fixMath;

[UpdateBefore(typeof(ApplyVelocitySystem))]
public class DirectAlongPathSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        Entities.ForEach(
            (Entity entity,
            DynamicBuffer<PathPosition> pathPositions,
            ref FixTranslation translation,
            ref Velocity velocity,
            ref MoveSpeed moveSpeed) =>
            {
                if (pathPositions.Length == 0)
                {
                    velocity.Value = fix3(0);
                    EntityManager.RemoveComponent<PathPosition>(entity);
                    return;
                }

                fix moveDist = moveSpeed.Value * Time.DeltaTime;

                fix3 a = translation.Value;
                fix3 b = translation.Value;
                fix3 v = fix3(0);
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
                        b = pathPositions[0].Position;
                    }

                    v = b - a;
                    vLength = length(v);
                }

                fix3 dir = (vLength == 0 ? fix3(0) : (v / vLength));

                fix3 targetPosition = a + (dir * min(moveDist, vLength));

                velocity.Value = (targetPosition - translation.Value) / Time.DeltaTime;
            });
    }
}
