using Unity.Entities;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Mathematics;

public struct TileCollisionEventData : IComponentData
{
    public Entity Entity;
    public int2 Tile;
}

//public class MovementSystemGroup : ComponentSystemGroup { }

//[UpdateInGroup(typeof(MovementSystemGroup))]
[UpdateAfter(typeof(ApplyVelocitySystem))]
public class ValidatePotentialNewTranslationSystem : SimComponentSystem
{
    EntityQuery _eventsEntityQuery;

    protected override void OnCreate()
    {
        base.OnCreate();

        _eventsEntityQuery = EntityManager.CreateEntityQuery(typeof(TileCollisionEventData));
    }

    protected override void OnUpdate()
    {
        // destroy events
        EntityManager.DestroyEntity(_eventsEntityQuery);

        Entities.ForEach((Entity entity,
            ref PotentialNewTranslation newTranslation,
            ref FixTranslation translation,
            ref Velocity velocity) =>
        {
            int2 nextTile = Helpers.GetTile(newTranslation.Value);
            int2 currentTile = Helpers.GetTile(translation.Value);

            if (!nextTile.Equals(currentTile))
            {
                var tileFlags = EntityManager.GetComponentData<TileFlagComponent>(CommonReads.GetTileEntity(Accessor, nextTile));

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
                        Tile = nextTile
                    });
                }
            }
        });
    }

    private static void KillVelocityInDirection(ref Velocity velocity, int2 dir)
    {
        fix3 vel = velocity.Value;
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

/*
public partial class CommonReads
{
    public static bool DoesTileRespectFilters(ISimWorldReadAccessor accessor, Entity tile, TileFilterFlags filter)
    {
        if (accessor.TryGetBufferReadOnly(tile, out DynamicBuffer<TileAddonReference> tileAddons) && tileAddons.Length > 0)
        {
            foreach (TileAddonReference addon in tileAddons)
            {
                if ((filter & TileFilterFlags.Navigable) != 0)
                {
                    if (accessor.HasComponent<SolidWallTag>(addon.Value))
                    {
                        return false;
                    }
                }

                if ((filter & TileFilterFlags.NonNavigable) != 0)
                {
                    if (!accessor.HasComponent<SolidWallTag>(addon.Value))
                    {
                        return false;
                    }
                }

                if ((filter & TileFilterFlags.Inoccupied) != 0)
                {
                    if (accessor.HasComponent<Occupied>(addon.Value))
                    {
                        return false;
                    }
                }

                if ((filter & TileFilterFlags.Occupied) != 0)
                {
                    if (!accessor.HasComponent<Occupied>(addon.Value))
                    {
                        return false;
                    }
                }

                if ((filter & TileFilterFlags.Ascendable) != 0)
                {
                    if (!accessor.HasComponent<AscendableTag>(addon.Value))
                    {
                        return false;
                    }
                }
            }
        }
        else
        {
            // No AddOn on tile (empty tile)
            // Does the action support empty tile ?
            if ((filter & TileFilterFlags.NotEmpty) != 0)
            {
                return false;
            }
        }

        return true;
    }
}
*/