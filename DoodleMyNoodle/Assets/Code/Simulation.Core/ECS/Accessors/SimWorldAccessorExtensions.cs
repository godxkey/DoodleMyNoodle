using Unity.Entities;

public static class SimWorldAccessorExtensions
{
    public static void SetOrAddComponentData<T>(this ISimWorldReadWriteAccessor accessor, Entity entity, in T componentData)
         where T : struct, IComponentData
    {
        if (accessor.HasComponent<T>(entity))
        {
            accessor.SetComponentData<T>(entity, componentData);
        }
        else
        {
            accessor.AddComponentData<T>(entity, componentData);
        }
    }


    public static bool TryGetComponentData<T>(this ISimWorldReadAccessor accessor, Entity entity, out T componentData)
         where T : struct, IComponentData
    {
        if (accessor.HasComponent<T>(entity))
        {
            componentData = accessor.GetComponentData<T>(entity);
            return true;
        }

        componentData = default;
        return false;
    }

    public static bool TryGetBuffer<T>(this ISimWorldReadAccessor accessor, Entity entity, out DynamicBuffer<T> buffer)
         where T : struct, IBufferElementData
    {
        if (accessor.HasComponent<T>(entity))
        {
            buffer = accessor.GetBuffer<T>(entity);
            return true;
        }

        buffer = default;
        return false;
    }
}