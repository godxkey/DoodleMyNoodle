using Unity.Entities;

public static class SimWorldAccessorExtensions
{
    public static void SetOrAddComponent<T>(this ISimWorldReadWriteAccessor accessor, Entity entity, in T componentData)
         where T : struct, IComponentData
    {
        if (accessor.HasComponent<T>(entity))
        {
            accessor.SetComponent<T>(entity, componentData);
        }
        else
        {
            accessor.AddComponent<T>(entity, componentData);
        }
    }

    public static bool TryGetComponent<T>(this ISimWorldReadAccessor accessor, Entity entity, out T componentData)
         where T : struct, IComponentData
    {
        if (accessor.HasComponent<T>(entity))
        {
            componentData = accessor.GetComponent<T>(entity);
            return true;
        }

        componentData = default;
        return false;
    }

    public static bool TryGetBuffer<T>(this ISimWorldReadWriteAccessor accessor, Entity entity, out DynamicBuffer<T> buffer)
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

    public static bool TryGetBufferReadOnly<T>(this ISimWorldReadAccessor accessor, Entity entity, out DynamicBuffer<T> buffer)
         where T : struct, IBufferElementData
    {
        if (accessor.HasComponent<T>(entity))
        {
            buffer = accessor.GetBufferReadOnly<T>(entity);
            return true;
        }

        buffer = default;
        return false;
    }

    public static bool TryGetSingleton<T>(this ISimWorldReadAccessor accessor, out T entity)
             where T : struct, IComponentData
    {
        if (accessor.HasSingleton<T>())
        {
            entity = accessor.GetSingleton<T>();
            return true;
        }

        entity = default;
        return false;
    }

    public static bool TryGetSingletonEntity<T>(this ISimWorldReadAccessor accessor, out Entity entity)
         where T : struct, IComponentData
    {
        if (accessor.HasSingleton<T>())
        {
            entity = accessor.GetSingletonEntity<T>();
            return true;
        }

        entity = Entity.Null;
        return false;
    }

    public static void SetOrCreateSingleton<T>(this ISimWorldReadWriteAccessor accessor, in T componentData)
        where T : struct, IComponentData
    {
        if (!accessor.HasSingleton<T>())
        {
            accessor.CreateNamedSingleton<T>();
        }

        accessor.SetSingleton(componentData);
    }

    public static T GetOrCreateSingleton<T>(this ISimWorldReadWriteAccessor accessor)
        where T : struct, IComponentData
    {
        if (!accessor.HasSingleton<T>())
        {
            accessor.CreateNamedSingleton<T>();
        }

        return accessor.GetSingleton<T>();
    }

    private static Entity CreateNamedSingleton<T>(this ISimWorldWriteAccessor accessor)
        where T : struct, IComponentData
    {
        var singletonEntity = accessor.CreateEntity(typeof(T));

#if UNITY_EDITOR
        accessor.SetName(singletonEntity, typeof(T).Name);
#endif

        return singletonEntity;
    }

    public static Entity CreateEventEntity<T>(this ISimWorldReadWriteAccessor entityManager) 
        where T : struct, IComponentData
    {
        return entityManager.CreateEntity(ComponentType.ReadWrite<T>());
    }

    public static Entity CreateEventEntity<T>(this ISimWorldReadWriteAccessor accessor, T data)
         where T : struct, IComponentData
    {
        Entity e = accessor.CreateEventEntity<T>();
        accessor.SetComponent(e, data);
        return e;
    }
}