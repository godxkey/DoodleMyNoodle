using Unity.Entities;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Mathematics;

[UpdateBefore(typeof(ApplyVelocitySystem))]
[UpdateAfter(typeof(CreatePathToDestinationSystem))]
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
                velocity.Value = fix3(0);
                PostUpdateCommands.RemoveComponent<PathPosition>(entity);
                return;
            }

            fix moveDist = moveSpeed.Value * deltaTime;

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

            velocity.Value = (targetPosition - translation.Value) / deltaTime;
        });
    }

    private void TryCancelPathForEntity(Entity entity)
    {
        if (EntityManager.HasComponent<PathPosition>(entity))
        {
            fix3 entityPos = EntityManager.GetComponentData<FixTranslation>(entity);
            fix3 newDestination = Helpers.GetTileCenter(entityPos);
            EntityManager.SetOrAddComponentData(entity, new Destination() { Value = newDestination });
        }
    }
}

[UpdateAfter(typeof(ApplyPotentialNewTranslationSystem))]
public class ResetDestinationOnCollideSystem : SimComponentSystem
{
    protected override void OnUpdate()
    {
        // if entities collide when moving along path, reset their destination
        Entities.ForEach((ref TileCollisionEventData collisionEvent) =>
        {
            if (EntityManager.HasComponent<PathPosition>(collisionEvent.Entity))
            {
                fix3 entityPos = EntityManager.GetComponentData<FixTranslation>(collisionEvent.Entity).Value;
                fix3 newDestination = Helpers.GetTileCenter(entityPos);
                EntityManager.SetOrAddComponentData(collisionEvent.Entity, new Destination() { Value = newDestination });
            }
        });
    }
}