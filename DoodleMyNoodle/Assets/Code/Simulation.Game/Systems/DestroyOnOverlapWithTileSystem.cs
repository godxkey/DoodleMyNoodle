using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using System;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(StepPhysicsWorldSystem)), UpdateBefore(typeof(EndFramePhysicsSystem))]
public class DestroyOnOverlapWithTileSystem : SimSystemBase
{
    private List<Entity> _toDestroy = new List<Entity>();

    protected override void OnUpdate()
    {
        Entities
           .WithoutBurst()
           .WithStructuralChanges()
           .ForEach((Entity entity, in DestroyOnOverlapWithTileTag destroyOnOverlapWithTileSystem, in FixTranslation fixTranslation) =>
           {
               Entity tileEntity = CommonReads.GetTileEntity(Accessor, Helpers.GetTile(fixTranslation));

               if (tileEntity != Entity.Null && EntityManager.TryGetComponentData(tileEntity, out TileFlagComponent tileFlagComponent))
               {
                   if (!tileFlagComponent.IsEmpty && !tileFlagComponent.IsLadder)
                   {
                       _toDestroy.Add(entity);
                   }
               }
           }).Run();

        foreach (var entity in _toDestroy)
        {
            EntityManager.DestroyEntity(entity);
        }

        _toDestroy.Clear();
    }
}