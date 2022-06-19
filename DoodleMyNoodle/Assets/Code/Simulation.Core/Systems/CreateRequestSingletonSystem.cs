using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using System;

public struct SingletonBuffersTag : IComponentData { }
public interface ISingletonBufferElementData : IBufferElementData { }

[UpdateInGroup(typeof(InitializationSystemGroup))]
[AlwaysUpdateSystem]
public class CreateRequestSingletonSystem : SimSystemBase
{
    public static ComponentType[] SystemRequestTypes;

    private EntityArchetype _singletonArchetype;

    protected override void OnCreate()
    {
        base.OnCreate();

        List<ComponentType> types = new List<ComponentType>();

        types.Add(typeof(SingletonBuffersTag));

        foreach (var item in TypeManager.AllTypes)
        {
            if (typeof(ISingletonBufferElementData).IsAssignableFrom(item.Type) && item.Category == TypeManager.TypeCategory.BufferData)
            {
                types.Add(new ComponentType(item.Type));
            }
        }

        types.Sort((a, b) => a.GetManagedType().Name.CompareTo(b.GetManagedType().Name));

        _singletonArchetype = EntityManager.CreateArchetype(types.ToArray());
    }

    protected override void OnUpdate()
    {
        if (!HasSingleton<SingletonBuffersTag>())
        {
            // create singleton
            var entity = EntityManager.CreateEntity(_singletonArchetype);
#if UNITY_EDITOR
            EntityManager.SetName(entity, "Singleton Data");
#endif
        }
        else if (_singletonArchetype.ChunkCount == 0)
        {
            // adjust singleton
            var entity = GetSingletonEntity<SingletonBuffersTag>();
            EntityManager.SetArchetype(entity, _singletonArchetype);
        }
    }
}