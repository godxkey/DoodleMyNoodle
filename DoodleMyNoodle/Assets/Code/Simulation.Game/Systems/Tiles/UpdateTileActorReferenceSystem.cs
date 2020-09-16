using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngineX;
using static fixMath;
using static Unity.Mathematics.math;

public struct TileActorSystemState : ISystemStateComponentData
{
    public int2 Tile;
}

public struct TileActorOverlapBeginEventData : IComponentData
{
    public Entity TileActorA;
    public Entity TileActorB;
    public int2 Tile;
    public Entity TileEntity;
}

public struct TileActorOverlapEndEventData : IComponentData
{
    public Entity TileActorA;
    public Entity TileActorB;
    public int2 Tile;
    public Entity TileEntity;
}

[UpdateAfter(typeof(ApplyPotentialNewTranslationSystem))]
[UpdateInGroup(typeof(MovementSystemGroup))]
public class UpdateTileActorReferenceSystem : SimJobComponentSystem
{
    EntityQuery _beginEventsGroup;
    EntityQuery _endEventsGroup;

    protected override void OnCreate()
    {
        base.OnCreate();

        _beginEventsGroup = EntityManager.CreateEntityQuery(typeof(TileActorOverlapBeginEventData));
        _endEventsGroup = EntityManager.CreateEntityQuery(typeof(TileActorOverlapEndEventData));

        RequireSingletonForUpdate<GridInfo>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        
        _beginEventsGroup.Dispose();
        _endEventsGroup.Dispose();
    }

    protected override JobHandle OnUpdate(JobHandle deps)
    {
        EntityCommandBuffer ecb = new EntityCommandBuffer(Unity.Collections.Allocator.Temp);

        // destroy events
        EntityManager.DestroyEntity(_beginEventsGroup);
        EntityManager.DestroyEntity(_endEventsGroup);

        deps.Complete();

        // Add 'TileActorSystemState' to all new entities
        Entities
           .WithAll<TileActorTag>()
           .WithNone<TileActorSystemState>()
           .WithoutBurst()
           .WithStructuralChanges()
           .ForEach((Entity entity, in FixTranslation translation) =>
           {
               // Add 'TileActorSystemState'
               int2 tile = Helpers.GetTile(translation);

               ecb.AddComponent(entity, new TileActorSystemState() { Tile = tile });
               AddToTile(entity, tile, ecb);
           }).Run();

        // Update tile for all tile actors
        Entities
            .WithAll<TileActorTag>()
            .WithChangeFilter<FixTranslation>()
            .WithoutBurst() // run on main thread
            .WithStructuralChanges()
            .ForEach((Entity entity, ref TileActorSystemState systemState, in FixTranslation translation) =>
            {
                // Check if tile changed
                int2 newTile = Helpers.GetTile(translation);

                if (!newTile.Equals(systemState.Tile))
                {
                    RemoveFromTile(entity, systemState.Tile, ecb);
                    AddToTile(entity, newTile, ecb);

                    // note: I don't know why, but setting the systemState's value directly here (+ using 'ref' in the ForEach param)
                    //       doesn't update the component value ...
                    EntityManager.SetComponentData(entity, new TileActorSystemState() { Tile = newTile });
                }
            }).Run();

        // Remove destroyed actors from tiles
        Entities
            .WithNone<TileActorTag>()
            .WithoutBurst()
            .WithStructuralChanges()
            .ForEach((Entity entity, in TileActorSystemState systemState) =>
            {
                RemoveFromTile(entity, systemState.Tile, ecb);
                ecb.RemoveComponent<TileActorSystemState>(entity);
            }).Run();

        ecb.Playback(EntityManager);

        return deps;
    }

    private void AddToTile(Entity entity, int2 tile, EntityCommandBuffer ecb)
    {
        Entity tileEntity = CommonReads.GetTileEntity(Accessor, tile);
        if (tileEntity != Entity.Null)
        {
            var buffer = EntityManager.GetBuffer<TileActorReference>(tileEntity);

            // fire 'trigger enter' events for each actor already on tile
            foreach (var actor in buffer)
            {
                ecb.CreateEventEntity(new TileActorOverlapBeginEventData()
                {
                    Tile = tile,
                    TileActorA = entity,
                    TileActorB = actor,
                });
            }

            // add entity to tile buffer
            buffer.Add(entity);
        }
    }

    private void RemoveFromTile(Entity entity, int2 tile, EntityCommandBuffer ecb)
    {
        Entity tileEntity = CommonReads.GetTileEntity(Accessor, tile);
        if (tileEntity != Entity.Null)
        {
            var buffer = EntityManager.GetBuffer<TileActorReference>(tileEntity);

            // remove entity from tile buffer
            buffer.RemoveFirst(entity);

            // fire 'trigger exit' events for each remaining actor on the tile
            foreach (var actor in buffer)
            {
                ecb.CreateEventEntity(new TileActorOverlapEndEventData()
                {
                    Tile = tile,
                    TileActorA = entity,
                    TileActorB = actor,
                });
            }

        }
    }
}