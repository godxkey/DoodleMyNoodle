using Unity.Entities;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Mathematics;

[UpdateBefore(typeof(ApplyVelocitySystem))]
[UpdateAfter(typeof(CreatePathToDestinationSystem))]
[UpdateInGroup(typeof(MovementSystemGroup))]
public class DirectAlongPathSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        // if entities collide when moving along path, reset their destination
        Entities.ForEach((ref TileCollisionEventData collisionEvent) =>
        {
            TryCancelPathForEntity(collisionEvent.Entity);
        });

        // if entities teleports when moving along path, reset their destination
        Entities.ForEach((ref TeleportEventData teleportEvent) =>
        {
            TryCancelPathForEntity(teleportEvent.Entity);
        });

        var deltaTime = Time.DeltaTime;

        // move along path
        Entities.ForEach(
            (Entity entity,
            DynamicBuffer<PathPosition> pathPositions,
            ref Velocity velocity,
            ref FixTranslation translation,
            ref MoveSpeed moveSpeed) =>
        {
            if (pathPositions.Length == 0)
            {
                velocity.Value = fix2(0);
                PostUpdateCommands.RemoveComponent<PathPosition>(entity);
                return;
            }

            fix moveDist = moveSpeed.Value * deltaTime;

            fix2 a = translation.Value;
            fix2 b = translation.Value;
            fix2 v = fix2(0);
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

            fix2 dir = (vLength == 0 ? fix2(0) : (v / vLength));
            fix2 targetPosition = a + (dir * min(moveDist, vLength));

            velocity.Value = (targetPosition - translation.Value) / deltaTime;
        });
    }

    private void TryCancelPathForEntity(Entity entity)
    {
        if (EntityManager.HasComponent<PathPosition>(entity))
        {
            fix2 entityPos = EntityManager.GetComponentData<FixTranslation>(entity);
            fix2 newDestination = Helpers.GetTileCenter(entityPos);
            EntityManager.SetOrAddComponentData(entity, new Destination() { Value = newDestination });
        }
    }
}
