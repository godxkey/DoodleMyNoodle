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
        // reset grounded
        Entities.ForEach((ref Grounded grounded) =>
        {
            grounded = false;
        }).Schedule();

        Dependency = new SetGroundedOnEntitiesThatCollideWithStatic()
        {
            Velocities = GetComponentDataFromEntity<PhysicsVelocity>(isReadOnly: true),
            Grounded = GetComponentDataFromEntity<Grounded>(isReadOnly: false),
        }.Schedule(_stepPhysicsWorldSystem.Simulation, ref _physicsWorldSystem.PhysicsWorld, Dependency);

        _endFramePhysicsSystem.HandlesToWaitFor.Add(Dependency);
    }

    struct SetGroundedOnEntitiesThatCollideWithStatic : ICollisionEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<PhysicsVelocity> Velocities;
        public ComponentDataFromEntity<Grounded> Grounded;

        public void Execute(CollisionEvent collisionEvent)
        {
            ProcessPair(collisionEvent.EntityA, collisionEvent.EntityB);
            ProcessPair(collisionEvent.EntityB, collisionEvent.EntityA);
        }

        private void ProcessPair(Entity entityA, Entity entityB)
        {
            if (Grounded.HasComponent(entityA) && !Velocities.HasComponent(entityB))
            {
                Grounded[entityA] = true;
            }
        }
    }
}