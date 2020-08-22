using Boo.Lang;
using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;
using static fixMath;
using static Unity.Mathematics.math;

public struct TeleportRequestSingletonBufferElement : IBufferElementData
{
    public Entity Entity;
    public fix3 Destination;
}

public struct TeleportEventData : IComponentData
{
    public Entity Entity;
    public fix3 Destination;
}

[UpdateAfter(typeof(ApplyVelocitySystem))]
[UpdateBefore(typeof(ValidatePotentialNewTranslationSystem))]
public class TeleportSystem : SimComponentSystem
{
    private EntityQuery _singletonQuery;
    private EntityQuery _eventGroup;

    public DynamicBuffer<TeleportRequestSingletonBufferElement> GetRequestBuffer()
    {
        // create singleton if necessary
        if (_singletonQuery.IsEmptyIgnoreFilter)
        {
            EntityManager.CreateEntity(typeof(TeleportRequestSingletonBufferElement));
        }

        return EntityManager.GetBuffer<TeleportRequestSingletonBufferElement>(_singletonQuery.GetSingletonEntity());
    }

    protected override void OnCreate()
    {
        base.OnCreate();
        _singletonQuery = EntityManager.CreateEntityQuery(typeof(TeleportRequestSingletonBufferElement));
        _eventGroup = EntityManager.CreateEntityQuery(typeof(TeleportEventData));
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _singletonQuery.Dispose();
        _eventGroup.Dispose();
    }

    protected override void OnUpdate()
    {
        // clear previous events
        EntityManager.DestroyEntity(_eventGroup);

        // process requests
        var requestBuffer = GetRequestBuffer();

        if (requestBuffer.Length > 0)
        {
            var requests = requestBuffer.ToNativeArray(Allocator.Temp);
            requestBuffer.Clear();

            foreach (var request in requests)
            {
                Teleport(request);
            }
        }

    }

    private void Teleport(TeleportRequestSingletonBufferElement request)
    {
        if (!EntityManager.Exists(request.Entity))
        {
            Log.Info($"Teleport request on entity {request.Entity} to position {request.Destination} " +
                $"will be ignored because the entity does not exist anymore");
            return;
        }

        if (!EntityManager.HasComponent<PotentialNewTranslation>(request.Entity))
        {
            Log.Info($"Teleport request on entity {request.Entity} to position {request.Destination} " +
                $"will be ignored because the entity does not have a {nameof(PotentialNewTranslation)}.");
            return;
        }

        EntityManager.SetComponentData<PotentialNewTranslation>(request.Entity, request.Destination);

        EntityManager.CreateEventEntity(new TeleportEventData()
        {
            Entity = request.Entity,
            Destination = request.Destination
        });
    }
}

internal partial class CommonWrites
{
    public static void RequestTeleport(ISimWorldReadWriteAccessor accessor, Entity entity, int2 destination)
    {
        RequestTeleport(accessor, entity, fix3(destination, 0));
    }

    public static void RequestTeleport(ISimWorldReadWriteAccessor accessor, Entity entity, fix3 destination)
    {
        var requests = accessor.GetExistingSystem<TeleportSystem>().GetRequestBuffer();
        requests.Add(new TeleportRequestSingletonBufferElement()
        {
            Entity = entity,
            Destination = destination
        });
    }
}