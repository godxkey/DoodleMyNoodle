﻿using CCC.Fix2D;
using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngineX;

[UpdateInGroup(typeof(MovementSystemGroup))]
public class UpdateNavAgentFootingSystem : SimGameSystemBase
{
    private PhysicsWorldSystem _physicsWorldSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        _physicsWorldSystem = World.GetOrCreateSystem<PhysicsWorldSystem>();
        RequireSingletonForUpdate<GridInfo>();
    }

    protected override void OnUpdate()
    {
        TileWorld tileWorld = CommonReads.GetTileWorld(Accessor);
        PhysicsWorld physicsWorld = _physicsWorldSystem.PhysicsWorld;

        Entities
            .WithReadOnly(tileWorld)
            .WithReadOnly(physicsWorld)
            .ForEach((Entity entity, ref NavAgentFootingState footing, in FixTranslation fixTranslation, in PhysicsColliderBlob colliderRef, in PhysicsVelocity velocity) =>
            {
                if (!colliderRef.Collider.IsCreated)
                    return;

                ref Collider collider = ref colliderRef.Collider.Value;
                fix pawnRadius = (fix)collider.Radius;
                fix pawnFeetXOffset = pawnRadius * (fix)0.8f;
                fix2 belowLeftFoot = new fix2(fixTranslation.Value.x - pawnFeetXOffset, fixTranslation.Value.y - pawnRadius - (fix)0.05);
                fix2 belowRightFoot = new fix2(fixTranslation.Value.x + pawnFeetXOffset, fixTranslation.Value.y - pawnRadius - (fix)0.05);
                fix2 belowLongDick = new fix2(fixTranslation.Value.x, fixTranslation.Value.y - pawnRadius - (fix)0.05);


                // GOTO Ladder IF on ladder && (already has footing on ladder || already has footing on ground)
                if (tileWorld.GetFlags(Helpers.GetTile(fixTranslation)).IsLadder
                    && (footing.Value == NavAgentFooting.Ladder || footing.Value == NavAgentFooting.Ground))
                {
                    footing.Value = NavAgentFooting.Ladder;
                }
                else
                {
                    OverlapPointInput detectTerrain = new OverlapPointInput()
                    {
                        Filter = SimulationGameConstants.Physics.CharacterFilter.Data,
                    };

                    OverlapPointInput detectTerrainLeftFoot = detectTerrain;
                    detectTerrainLeftFoot.Position = (float2)belowLeftFoot;
                    OverlapPointInput detectTerrainLongDick = detectTerrain;
                    detectTerrainLongDick.Position = (float2)belowLongDick;
                    OverlapPointInput detectTerrainRightFoot = detectTerrain;
                    detectTerrainRightFoot.Position = (float2)belowRightFoot;

                    // GOTO Ground IF above terrain && (previously grounded || previously ladder || previously airControl and not jumping || velocity is low)
                    if ((physicsWorld.OverlapPoint(detectTerrainLeftFoot) || physicsWorld.OverlapPoint(detectTerrainLongDick) || physicsWorld.OverlapPoint(detectTerrainRightFoot))
                      && (footing.Value == NavAgentFooting.Ground
                          || footing.Value == NavAgentFooting.Ladder
                          || (footing.Value == NavAgentFooting.AirControl && velocity.Linear.y <= (fix)0.5)
                          || velocity.Linear.lengthSquared < 4))
                    {
                        footing.Value = NavAgentFooting.Ground;
                    }
                    else
                    {
                        // GOTO air control IF in mid-air && was not in None
                        if (footing.Value != NavAgentFooting.None)
                        {
                            footing.Value = NavAgentFooting.AirControl;
                        }
                    }
                }
            }).Run();
    }
}