using System;
using Unity.Entities;
using UnityEngine;
using static fixMath;
using System.Collections.Generic;
using CCC.Fix2D;


public partial class DestroyDeadEntitiesSystem : SimGameSystemBase
{
    private EndSimulationEntityCommandBufferSystem _ecbSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _ecbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        var ecb = _ecbSystem.CreateCommandBuffer();
        fix destroyTime = Time.ElapsedTime - SimulationGameConstants.DeadEntityDestroyDelay;
        Entities.ForEach((Entity entity, in DeadTimestamp deadTimestamp) =>
        {
            if(deadTimestamp.TimeOfDeath < destroyTime)
            {
                ecb.DestroyEntity(entity);
            }
        }).Schedule();

        _ecbSystem.AddJobHandleForProducer(Dependency);
    }
}