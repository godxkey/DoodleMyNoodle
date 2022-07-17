using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;
using System;

public struct Flying : IComponentData
{
    public bool Value;

    public static implicit operator bool(Flying val) => val.Value;
    public static implicit operator Flying(bool val) => new Flying() { Value = val };
}

public struct Grounded : IComponentData
{
    public bool Value;

    public static implicit operator bool(Grounded val) => val.Value;
    public static implicit operator Grounded(bool val) => new Grounded() { Value = val };
}

public struct ResetGroundedFlag : IComponentData
{
    public bool Value;

    public static implicit operator bool(ResetGroundedFlag val) => val.Value;
    public static implicit operator ResetGroundedFlag(bool val) => new ResetGroundedFlag() { Value = val };
}

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(StepPhysicsWorldSystem)), UpdateBefore(typeof(EndFramePhysicsSystem))]
public partial class UpdateGroundedSystem : SimGameSystemBase
{
    private StepPhysicsWorldSystem _stepPhysicsWorldSystem;
    private PhysicsWorldSystem _physicsWorldSystem;
    private EndFramePhysicsSystem _endFramePhysicsSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _stepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorldSystem>();
        _physicsWorldSystem = World.GetOrCreateSystem<PhysicsWorldSystem>();
        _endFramePhysicsSystem = World.GetOrCreateSystem<EndFramePhysicsSystem>();
    }

    protected override void OnUpdate()
    {
        // schedule reset grounded
        Entities.ForEach((ref ResetGroundedFlag resetGrounded) =>
        {
            resetGrounded = true;
        }).Schedule();

        Dependency = new SetGroundedOnEntitiesThatCollideWithStatic()
        {
            Velocities = GetComponentDataFromEntity<PhysicsVelocity>(isReadOnly: true),
            ResetGroundedFlag = GetComponentDataFromEntity<ResetGroundedFlag>(isReadOnly: false),
            Grounded = GetComponentDataFromEntity<Grounded>(isReadOnly: false),
        }.Schedule(_stepPhysicsWorldSystem.Simulation, ref _physicsWorldSystem.PhysicsWorld, Dependency);

        _endFramePhysicsSystem.HandlesToWaitFor.Add(Dependency);

        Entities.ForEach((ref Grounded grounded, in ResetGroundedFlag resetGrounded) =>
        {
            if (resetGrounded)
            {
                grounded.Value = false;
            }
        }).Schedule();
    }

    struct SetGroundedOnEntitiesThatCollideWithStatic : ICollisionEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<PhysicsVelocity> Velocities;
        public ComponentDataFromEntity<ResetGroundedFlag> ResetGroundedFlag;
        public ComponentDataFromEntity<Grounded> Grounded;

        public void Execute(CollisionEvent collisionEvent)
        {
            ProcessPair(collisionEvent.EntityA, collisionEvent.EntityB);
            ProcessPair(collisionEvent.EntityB, collisionEvent.EntityA);
        }

        private void ProcessPair(Entity entityA, Entity entityB)
        {
            if (// entityA has component
                Grounded.HasComponent(entityA) && 

                // entityB is static
                !Velocities.HasComponent(entityB)/* && 
                
                // entityA was grounded last frame OR speed is low enough to get grip back on ground
                (Grounded[entityA] || abs(Velocities[entityA].Linear.x) < (fix)0.01)*/)
            {
                Grounded[entityA] = true;
                ResetGroundedFlag[entityA] = false;
            }
        }
    }
}