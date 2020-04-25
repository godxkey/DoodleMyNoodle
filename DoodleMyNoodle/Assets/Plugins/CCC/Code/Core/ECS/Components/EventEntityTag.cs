using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[Serializable]
public struct EventEntityTag : IComponentData
{
}

public static class EntityManagerEventExtensions
{
    public static Entity CreateEventEntity<T>(this EntityManager entityManager)
         where T : struct, IComponentData
    {
        return entityManager.CreateEntity(typeof(EventEntityTag), ComponentType.ReadWrite<T>());
    }

    public static Entity CreateEventEntity<T>(this EntityManager entityManager, T data)
         where T : struct, IComponentData
    {
        Entity e = entityManager.CreateEventEntity<T>();
        entityManager.SetComponentData(e, data);
        return e;
    }
}