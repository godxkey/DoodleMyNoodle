using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

internal static partial class CommonWrites
{
    public static void DestroyEndOfTick(ISimGameWorldReadWriteAccessor accessor, Entity entity)
    {
        accessor.GetExistingSystem<EndSimulationEntityCommandBufferSystem>().CreateCommandBuffer().DestroyEntity(entity);
    }
}