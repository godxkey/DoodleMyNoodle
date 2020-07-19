using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

// THIS CLASS CANNOT BE USED AT THE MOMENT (generic method crashes the editor in Entities 0.10.0)

public abstract class CCCSystemBase : SystemBase
{
    public Entity CreateSingleton<T>()
        where T : struct, IComponentData
    {
        var singletonEntity = EntityManager.CreateEntity(typeof(T));

#if UNITY_EDITOR
        EntityManager.SetName(singletonEntity, typeof(T).Name);
#endif

        return singletonEntity;
    }

    public Entity CreateSingleton<T>(T value)
        where T : struct, IComponentData
    {
        var singletonEntity = EntityManager.CreateEntity(typeof(T));

#if UNITY_EDITOR
        EntityManager.SetName(singletonEntity, typeof(T).Name);
#endif

        SetSingleton(value);

        return singletonEntity;
    }

    public void SetOrCreateSingleton<T>(in T componentData)
        where T : struct, IComponentData
    {
        if (!HasSingleton<T>())
        {
            CreateSingleton(componentData);
        }
        else
        {
            SetSingleton(componentData);
        }
    }

    public T GetOrCreateSingleton<T>(T defaultValue = default)
        where T : struct, IComponentData
    {
        if (HasSingleton<T>())
        {
            return GetSingleton<T>();
        }
        else
        {
            CreateSingleton(defaultValue);
            return defaultValue;
        }
    }

    public bool TryGetSingleton<T>(out T componentData)
        where T : struct, IComponentData
    {
        if (HasSingleton<T>())
        {
            componentData = GetSingleton<T>();
            return true;
        }

        componentData = default;
        return false;
    }

    public void DestroySingleton<T>()
        where T : struct, IComponentData
    {
        Entity e = GetSingletonEntity<T>();
        EntityManager.DestroyEntity(e);
    }

    public bool TryGetComponent<T>(Entity entity, out T value) where T : struct, IComponentData
    {
        if (HasComponent<T>(entity))
        {
            value = GetComponent<T>(entity);
            return true;
        }
        else
        {
            value = default;
            return false;
        }
    }
}