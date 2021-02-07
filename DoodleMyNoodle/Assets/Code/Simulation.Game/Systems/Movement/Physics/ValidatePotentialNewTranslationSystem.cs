using Unity.Entities;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Mathematics;

public struct TileCollisionEventData : IComponentData
{
    public Entity Entity;
    public int2 Tile;
    public Entity TileEntity;
}

[UpdateInGroup(typeof(MovementSystemGroup))]
[UpdateAfter(typeof(ApplyVelocitySystem))]
public class ValidatePotentialNewTranslationSystem : SimComponentSystem
{
    EntityQuery _eventsGroup;

    protected override void OnCreate()
    {
        base.OnCreate();

        _eventsGroup = EntityManager.CreateEntityQuery(typeof(TileCollisionEventData));
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _eventsGroup.Dispose();
    }

    protected override void OnUpdate()
    {
        // destroy events
        EntityManager.DestroyEntity(_eventsGroup);

        Entities.ForEach((Entity entity,
            ref PotentialNewTranslation newTranslation,
            ref FixTranslation translation,
            ref Velocity velocity) =>
        {
            int2 nextTile = Helpers.GetTile(newTranslation.Value);
            int2 currentTile = Helpers.GetTile(translation.Value);

            if (!nextTile.Equals(currentTile))
            {
                var nextTileEntity = CommonReads.GetTileEntity(Accessor, nextTile);

                if (nextTileEntity != Entity.Null)
                {
                    var tileFlags = EntityManager.GetComponentData<TileFlagComponent>(nextTileEntity);

                    if (tileFlags.IsTerrain)
                    {
                        // collision!

                        // kill velocity
                        KillVelocityInDirection(ref velocity, dir: nextTile - currentTile);

                        // cancel movement
                        newTranslation.Value = translation.Value;

                        // create event
                        EntityManager.CreateEventEntity(new TileCollisionEventData()
                        {
                            Entity = entity,
                            Tile = nextTile,
                            TileEntity = nextTileEntity
                        });
                    }
                }
            }
        });
    }

    private static void KillVelocityInDirection(ref Velocity velocity, int2 dir)
    {
        fix2 vel = velocity.Value;
        if (dir.x > 0)
            vel.x = min(vel.x, 0);
        else if (dir.x < 0)
            vel.x = max(vel.x, 0);

        if (dir.y > 0)
            vel.y = min(vel.y, 0);
        else if (dir.y < 0)
            vel.y = max(vel.y, 0);

        velocity.Value = vel;
    }
}
