using CCC.Fix2D;
using System;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngineX;

[UpdateInGroup(typeof(MovementSystemGroup))]
public class UpdateNavAgentFootingSystem : SimSystemBase
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
                        Filter = SimulationGameConstants.Physics.CollideWithTerrainFilter.Data,
                    };

                    OverlapPointInput detectTerrainLeftFoot = detectTerrain;
                    detectTerrainLeftFoot.Position = (float2)belowLeftFoot;
                    OverlapPointInput detectTerrainRightFoot = detectTerrain;
                    detectTerrainRightFoot.Position = (float2)belowRightFoot;

                    // GOTO Ground IF above terrain && (previously grounded || previously ladder || previously airControl and not jumping || velocity is low)
                    if ((physicsWorld.OverlapPoint(detectTerrainLeftFoot) || physicsWorld.OverlapPoint(detectTerrainRightFoot))
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

        // depending on the agent's state, update the collider
        Entities
            .WithChangeFilter<NavAgentFootingState, Health>()
            .ForEach((ref PhysicsColliderBlob collider, in NavAgentFootingState footing, in NavAgentColliderRefs colliderRefs, in Health health) =>
            {
                UpdateAgentCollider(ref collider, footing, colliderRefs, alive: health > 0);
            }).Run();
        Entities
            .WithNone<Health>()
            .WithChangeFilter<NavAgentFootingState>()
            .ForEach((ref PhysicsColliderBlob collider, in NavAgentFootingState footing, in NavAgentColliderRefs colliderRefs) =>
            {
                UpdateAgentCollider(ref collider, footing, colliderRefs, alive: true);
            }).Run();
    }

    private static void UpdateAgentCollider(ref PhysicsColliderBlob collider, in NavAgentFootingState footing, in NavAgentColliderRefs colliderRefs, bool alive)
    {
        // When pawn is in the air, reduce friction so we don't break on walls
        if (!alive)
        {
            collider.Collider = colliderRefs.DeadCollider;
        }
        else
        {
            collider.Collider = footing.Value == NavAgentFooting.AirControl
                ? colliderRefs.AirControlCollider
                : colliderRefs.NormalCollider;
        }
    }
}
