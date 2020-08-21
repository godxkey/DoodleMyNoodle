using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

public static class EntityManagerEventExtensions
{
    public static Entity CreateEventEntity<T>(this EntityManager entityManager)
         where T : struct, IComponentData
    {
        return entityManager.CreateEntity(ComponentType.ReadWrite<T>());
    }

    public static Entity CreateEventEntity<T>(this EntityManager entityManager, T data)
         where T : struct, IComponentData
    {
        Entity e = entityManager.CreateEventEntity<T>();
        entityManager.SetComponentData(e, data);
        return e;
    }

    public static Entity CreateEventEntity<T>(this EntityCommandBuffer ecb)
         where T : struct, IComponentData
    {
        Entity entity = ecb.CreateEntity();
        ecb.AddComponent<T>(entity);
        return entity;
    }

    public static Entity CreateEventEntity<T>(this EntityCommandBuffer ecb, T data)
         where T : struct, IComponentData
    {
        Entity entity = ecb.CreateEntity();
        ecb.AddComponent<T>(entity, data);
        return entity;
    }
}