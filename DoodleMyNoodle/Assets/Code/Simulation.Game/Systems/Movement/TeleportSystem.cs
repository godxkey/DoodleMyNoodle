using CCC.Fix2D;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;

public struct TeleportRequestSingletonBufferElement : IBufferElementData
{
    public Entity Entity;
    public fix2 Destination;
    public bool ThroughPortal;
}

public struct TeleportEventData : IComponentData
{
    public Entity Entity;
    public fix2 Destination;
}

[UpdateInGroup(typeof(MovementSystemGroup))]
public class TeleportSystem : SimSystemBase
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

        if (!HasComponent<FixTranslation>(request.Entity))
        {
            Log.Info($"Teleport request on entity {request.Entity} to position {request.Destination} " +
                $"will be ignored because the entity does not have a {nameof(FixTranslation)}.");
            return;
        }

        SetComponent<FixTranslation>(request.Entity, request.Destination);
        if (request.ThroughPortal)
        {
            Accessor.AddComponent(request.Entity, new InsidePortalTag());
        }

        EntityManager.CreateEventEntity(new TeleportEventData()
        {
            Entity = request.Entity,
            Destination = request.Destination
        });
    }
}

internal partial class CommonWrites
{
    public static void RequestTeleport(ISimWorldReadWriteAccessor accessor, Entity entity, fix2 destination, bool throughPortal = false)
    {
        var requests = accessor.GetExistingSystem<TeleportSystem>().GetRequestBuffer();
        requests.Add(new TeleportRequestSingletonBufferElement()
        {
            Entity = entity,
            Destination = destination,
            ThroughPortal = throughPortal
        });
    }
}