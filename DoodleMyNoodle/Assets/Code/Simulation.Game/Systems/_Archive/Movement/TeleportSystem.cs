using CCC.Fix2D;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngineX;

public struct SystemRequestTeleport : ISingletonBufferElementData
{
    public Entity Entity;
    public fix2 Destination;
}

[UpdateInGroup(typeof(MovementSystemGroup))]
public class TeleportSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        // process requests
        var requestBuffer = GetSingletonBuffer<SystemRequestTeleport>();

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

    private void Teleport(SystemRequestTeleport request)
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
    }
}

internal partial class CommonWrites
{
    public static void RequestTeleport(ISimGameWorldReadWriteAccessor accessor, Entity entity, fix2 destination)
    {
        var requests = accessor.GetSingletonBuffer<SystemRequestTeleport>();
        requests.Add(new SystemRequestTeleport()
        {
            Entity = entity,
            Destination = destination
        });
    }
}