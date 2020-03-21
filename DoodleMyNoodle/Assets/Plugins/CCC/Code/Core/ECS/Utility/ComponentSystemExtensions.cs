
using Unity.Entities;

public static class ComponentSystemExtensions
{
    private static Entity CreateNamedSingleton<T>(this ComponentSystem componentSystem)
        where T : struct, IComponentData
    {
        var singletonEntity = componentSystem.EntityManager.CreateEntity(typeof(T));

#if UNITY_EDITOR
        componentSystem.EntityManager.SetName(singletonEntity, typeof(T).Name);
#endif

        return singletonEntity;
    }

    public static void SetOrCreateSingleton<T>(this ComponentSystem componentSystem, in T componentData)
        where T : struct, IComponentData
    {
        if (!componentSystem.HasSingleton<T>())
        {
            componentSystem.CreateNamedSingleton<T>();
        }

        componentSystem.SetSingleton(componentData);
    }

    public static T GetOrCreateSingleton<T>(this ComponentSystem componentSystem, T defaultValue = default)
        where T : struct, IComponentData
    {
        if (componentSystem.HasSingleton<T>())
        {
            return componentSystem.GetSingleton<T>();
        }
        else
        {
            var newEntity = componentSystem.CreateNamedSingleton<T>();
            componentSystem.EntityManager.SetComponentData(newEntity, defaultValue);
            return defaultValue;
        }
    }

    public static bool TryGetSingleton<T>(this ComponentSystem componentSystem, out T componentData)
        where T : struct, IComponentData
    {
        if (componentSystem.HasSingleton<T>())
        {
            componentData = componentSystem.GetSingleton<T>();
            return true;
        }

        componentData = default;
        return false;
    }
}