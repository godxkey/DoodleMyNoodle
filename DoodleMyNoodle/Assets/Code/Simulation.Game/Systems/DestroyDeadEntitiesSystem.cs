using System;
using Unity.Entities;
using UnityEngine;
using static fixMath;
using System.Collections.Generic;
using CCC.Fix2D;


public class DestroyDeadEntitiesSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        Entities
            .WithNone<DeadTag>()
            .WithStructuralChanges()
            .ForEach((Entity entity, ref Health health) =>
            {
                if (health.Value <= 0)
                {
                    if (EntityManager.HasComponent<DestroyOnDeath>(entity))
                    {
                        EntityManager.DestroyEntity(entity);
                    }
                    else
                    {
                        if (EntityManager.HasComponent<PhysicsGravity>(entity))
                        {
                            EntityManager.SetComponentData(entity, new PhysicsGravity() { Scale = 1 });
                        }
                        EntityManager.AddComponentData(entity, new DeadTag());
                    }
                }
            }).Run();
    }
}
