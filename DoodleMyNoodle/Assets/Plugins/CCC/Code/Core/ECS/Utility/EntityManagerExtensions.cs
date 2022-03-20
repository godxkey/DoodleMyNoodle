using Unity.Collections;
using Unity.Entities;

public static class EntityManagerExtensions
{
    public static bool TryGetBuffer<T>(this EntityManager entityManager, Entity entity, out DynamicBuffer<T> buffer)
         where T : struct, IBufferElementData
    {
        if (entityManager.HasComponent<T>(entity))
        {
            buffer = entityManager.GetBuffer<T>(entity);
            return true;
        }

        buffer = default;
        return false;
    }

    public static string GetNameSafe(this EntityManager entityManager, Entity entity)
    {
#if UNITY_EDITOR
        if (entity == Entity.Null)
            return "Entity.Null";
        if (!entityManager.Exists(entity))
            return $"{entity}(destroyed)";
        return entityManager.GetName(entity);
#else
        return entity.ToString();
#endif
    }

    public static DynamicBuffer<T> GetOrAddBuffer<T>(this EntityManager entityManager, Entity entity)
        where T : struct, IBufferElementData
    {
        if (entityManager.HasComponent<T>(entity))
        {
            return entityManager.GetBuffer<T>(entity);
        }
        else
        {
            return entityManager.AddBuffer<T>(entity);
        }
    }

    public static bool TryGetComponent<T>(this EntityManager entityManager, Entity entity, out T componentData)
         where T : struct, IComponentData
    {
        if (entityManager.HasComponent<T>(entity))
        {
            componentData = entityManager.GetComponentData<T>(entity);
            return true;
        }

        componentData = default;
        return false;
    }

    public static void SetOrAddComponent<T>(this EntityManager entityManager, Entity entity, T componentData)
         where T : struct, IComponentData
    {
        if (entityManager.HasComponent<T>(entity))
        {
            entityManager.SetComponentData(entity, componentData);
        }
        else
        {
            entityManager.AddComponentData(entity, componentData);
        }
    }
}