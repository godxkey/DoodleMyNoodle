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
            ref FixTranslation translation,
            ref Velocity velocity,
            ref PotentialNewTranslation newTranslation) =>
        {
            int2 tileDestinationPos = roundToInt(newTranslation.Value).xy;
            int2 currentTilePos = roundToInt(translation.Value).xy;

            if (!tileDestinationPos.Equals(currentTilePos))
            {
                if (!CommonReads.DoesTileRespectFilters(Accessor, CommonReads.GetTile(Accessor, tileDestinationPos), TileFilterFlags.Inoccupied | TileFilterFlags.Navigable))
                {
                    EntityManager.CreateEventEntity(new TileCollisionEventData() { Entity = entity, Tile = tileDestinationPos });

                    newTranslation.Value = translation.Value;
                    Accessor.SetOrAddComponentData(entity, new Destination { Value = roundToInt(newTranslation.Value) });
                }
            }
        });

        NativeHashMap<int2, Entity> reservedTiled = new NativeHashMap<int2, Entity>(128, Allocator.Temp);

        Entities.ForEach((Entity entity,
           ref FixTranslation translation,
           ref Velocity velocity,
           ref PotentialNewTranslation newTranslation) =>
        {
            int2 tileDestinationPos = roundToInt(newTranslation.Value).xy;

            // Already reserved
            if (reservedTiled.ContainsKey(tileDestinationPos))
            {
                EntityManager.CreateEventEntity(new TileCollisionEventData() { Entity = entity, Tile = tileDestinationPos });

                int2 currentTilePos = roundToInt(translation.Value).xy;
                newTranslation.Value = translation.Value;

                Accessor.SetOrAddComponentData(entity, new Destination { Value = roundToInt(newTranslation.Value) });

                reservedTiled.SetOrAdd(currentTilePos, entity);
            }
            else
            {
                reservedTiled.Add(tileDestinationPos, entity);
            }
        });
    }
}

public partial class CommonReads
{
    public static bool DoesTileRespectFilters(ISimWorldReadAccessor accessor, Entity tile, TileFilterFlags filter)
    {
        DynamicBuffer<EntityOnTile> tileAddons = accessor.GetBufferReadOnly<EntityOnTile>(tile);

        if (tileAddons.Length > 0)
        {
            foreach (EntityOnTile addon in tileAddons)
            {
                if ((filter & TileFilterFlags.Navigable) != 0)
                {
                    if (accessor.HasComponent<NonNavigable>(addon.TileEntity))
                    {
                        return false;
                    }
                }

                if ((filter & TileFilterFlags.NonNavigable) != 0)
                {
                    if (!accessor.HasComponent<NonNavigable>(addon.TileEntity))
                    {
                        return false;
                    }
                }

                if ((filter & TileFilterFlags.Inoccupied) != 0)
                {
                    if (accessor.HasComponent<Occupied>(addon.TileEntity))
                    {
                        return false;
                    }
                }

                if ((filter & TileFilterFlags.Occupied) != 0)
                {
                    if (!accessor.HasComponent<Occupied>(addon.TileEntity))
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
