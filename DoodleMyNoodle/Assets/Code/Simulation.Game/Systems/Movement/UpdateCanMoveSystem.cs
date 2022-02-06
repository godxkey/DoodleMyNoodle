﻿using Unity.Entities;
using CCC.Fix2D;
using Unity.Mathematics;
using Unity.Collections;

public class UpdateCanMoveSystem : SimGameSystemBase
{
    private PhysicsWorldSystem _physicsWorldSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _physicsWorldSystem = World.GetOrCreateSystem<PhysicsWorldSystem>();
    }

    protected override void OnUpdate()
    {
        // by default, everyone can move
        Entities.ForEach((ref CanMove canMove) =>
        {
            canMove = true;
        }).Run();


        // If there is a player group, stop all entities that touch the trigger zone
        if (HasSingleton<PlayerGroupDataTag>())
        {
            Entity playerGroupEntity = GetSingletonEntity<PlayerGroupDataTag>();
            fix2 playerGroupStopTriggerSize = GetComponent<PlayerGroupStopTriggerSize>(playerGroupEntity);
            fix2 playerGroupPosition = GetComponent<FixTranslation>(playerGroupEntity);
            fix2 min = new fix2(playerGroupPosition.x + SimulationGameConstants.CharacterRadius + (fix)0.05, playerGroupPosition.y - playerGroupStopTriggerSize.y / 2);
            fix2 max = min + playerGroupStopTriggerSize;

            OverlapAabbInput overlapAabbInput = new OverlapAabbInput()
            {
                Aabb = new Aabb((float2)min, (float2)max),
                Filter = SimulationGameConstants.Physics.CollideWithCharactersFilter.Data,
            };
            NativeList<int> touchedbodies = new NativeList<int>(Allocator.Temp);
            PhysicsWorld physicsWorld = _physicsWorldSystem.PhysicsWorld;
            if (physicsWorld.OverlapAabb(overlapAabbInput, touchedbodies))
            {
                SetComponent<CanMove>(playerGroupEntity, false);

                foreach (var bodyIndex in touchedbodies)
                {
                    var body = physicsWorld.AllBodies[bodyIndex];
                    if (HasComponent<CanMove>(body.Entity))
                    {
                        SetComponent<CanMove>(body.Entity, false);
                    }
                }
            }
            else
            {
                SetComponent<CanMove>(playerGroupEntity, true);
            }
        }
    }
}