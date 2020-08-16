using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using static Unity.Mathematics.math;
using static fixMath;
using Unity.Mathematics;
using Unity.Collections;
using UnityEngine.Profiling;

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
            int2 tileDestinationPos = Helpers.GetTile(newTranslation.Value);
            int2 currentTilePos = Helpers.GetTile(translation.Value);

            if (!tileDestinationPos.Equals(currentTilePos))
            {
                if (!CommonReads.DoesTileRespectFilters(Accessor, CommonReads.GetTileEntity(Accessor, tileDestinationPos), TileFilterFlags.Inoccupied | TileFilterFlags.Navigable))
                {
                    // create event
                    EntityManager.CreateEventEntity(new TileCollisionEventData()
                    {
                        Entity = entity,
                        Tile = tileDestinationPos
                    });

                    // cancel movement
                    newTranslation.Value = translation.Value;
                }
            }
        });

        NativeHashMap<int2, Entity> reservedTiled = new NativeHashMap<int2, Entity>(128, Allocator.Temp);

        Entities.ForEach((Entity entity,
            ref PotentialNewTranslation newTranslation,
            ref FixTranslation translation,
            ref Velocity velocity) =>
        {
            int2 tileDestinationPos = Helpers.GetTile(newTranslation.Value);

            // Already reserved
            if (reservedTiled.ContainsKey(tileDestinationPos))
            {
                // create event
                EntityManager.CreateEventEntity(new TileCollisionEventData()
                {
                    Entity = entity,
                    Tile = tileDestinationPos
                });

                // cancel movement
                int2 currentTilePos = Helpers.GetTile(translation);
                newTranslation.Value = translation.Value;

                // reserve CURRENT tile
                reservedTiled.SetOrAdd(currentTilePos, entity);
            }
            else
            {
                // reserve NEW tile
                reservedTiled.Add(tileDestinationPos, entity);
            }
        });
    }
}

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
