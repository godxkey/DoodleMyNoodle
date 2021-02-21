using Unity.Entities;
using static Unity.Mathematics.math;
using static fixMath;
using CCC.Fix2D;
using Unity.Mathematics;

[UpdateAfter(typeof(CreatePathToDestinationSystem))]
[UpdateInGroup(typeof(MovementSystemGroup))]
public class DirectAlongPathSystem : SimSystemBase
{
    EntityCommandBufferSystem _endSimECB;

    protected override void OnCreate()
    {
        base.OnCreate();

        _endSimECB = World.GetExistingSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        // if entities collide when moving along path, reset their destination
        Entities.ForEach((in TileCollisionEventData collisionEvent) =>
        {
            TryCancelPathForEntity(collisionEvent.Entity);
        }).WithoutBurst()
        .Run();

        // if entities teleports when moving along path, reset their destination
        Entities.ForEach((in TeleportEventData teleportEvent) =>
        {
            TryCancelPathForEntity(teleportEvent.Entity);
        }).WithoutBurst()
        .Run();

        var deltaTime = Time.DeltaTime;

        // move along path
        EntityCommandBuffer cmdBuffer = _endSimECB.CreateCommandBuffer();
        Entities
            .WithNone<MovingPlatformSettings>()
            .ForEach(
            (Entity entity,
            DynamicBuffer<PathPosition> pathPositions,
            ref PhysicsVelocity velocity,
            in FixTranslation translation,
            in MoveSpeed moveSpeed) =>
        {
            if (pathPositions.Length == 0)
            {
                velocity.Linear = fix2(0);
                cmdBuffer.RemoveComponent<PathPosition>(entity);
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
            fix2 targetVelocity = (targetPosition - translation.Value) / deltaTime;
            velocity.Linear = targetVelocity;// lerp(velocity, targetVelocity, deltaTime);
        }).Run();
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
