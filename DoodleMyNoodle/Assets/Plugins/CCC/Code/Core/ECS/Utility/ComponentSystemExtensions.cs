
using Unity.Entities;

public static class ComponentSystemExtensions
{
    public static void SetOrCreateSingleton<T>(this ComponentSystem componentSystem, in T componentData)
        where T : struct, IComponentData
    {
        if (!componentSystem.HasSingleton<T>())
        {
            componentSystem.EntityManager.CreateEntity(typeof(T));
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
            var newEntity = componentSystem.EntityManager.CreateEntity(typeof(T));
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