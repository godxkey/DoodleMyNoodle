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
        return entityManager.CreateEntity(typeof(EventEntityTag), ComponentType.ReadWrite<T>());
    }

    public static Entity CreateEventEntity<T>(this ISimWorldReadWriteAccessor accessor, T data)
         where T : struct, IComponentData
    {
        Entity e = accessor.CreateEventEntity<T>();
        accessor.SetComponentData(e, data);
        return e;
    }
}