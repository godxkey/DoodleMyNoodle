using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public abstract class CCCSystemBase : SystemBase
{
    protected Entity CreateSingleton<T>()
        where T : struct, IComponentData
    {
        var singletonEntity = EntityManager.CreateEntity(typeof(T));

#if UNITY_EDITOR
        EntityManager.SetName(singletonEntity, typeof(T).Name);
#endif

        return singletonEntity;
    }

    protected Entity CreateSingleton<T>(T value)
        where T : struct, IComponentData
    {
        var singletonEntity = EntityManager.CreateEntity(typeof(T));

#if UNITY_EDITOR
        EntityManager.SetName(singletonEntity, typeof(T).Name);
#endif

        SetSingleton(value);

        return singletonEntity;
    }

    protected void SetOrCreateSingleton<T>(in T componentData)
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

    protected T GetOrCreateSingleton<T>(T defaultValue = default)
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

    protected bool TryGetSingleton<T>(out T componentData)
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

    protected void DestroySingleton<T>()
        where T : struct, IComponentData
    {
        Entity e = GetSingletonEntity<T>();
        EntityManager.DestroyEntity(e);
    }

    protected bool TryGetComponent<T>(Entity entity, out T value) where T : struct, IComponentData
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