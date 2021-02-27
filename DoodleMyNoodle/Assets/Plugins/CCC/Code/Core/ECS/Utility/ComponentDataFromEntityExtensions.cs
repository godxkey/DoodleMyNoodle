using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public static class ComponentDataFromEntityExtensions
{
    public static bool TryGetComponent<T>(this ref ComponentDataFromEntity<T> componentDataFromEntity, Entity entity, out T component)
         where T : struct, IComponentData
    {
        if (componentDataFromEntity.HasComponent(entity))
        {
            component = componentDataFromEntity[entity];
            return true;
        }
        component = default;
        return false;
    }
}