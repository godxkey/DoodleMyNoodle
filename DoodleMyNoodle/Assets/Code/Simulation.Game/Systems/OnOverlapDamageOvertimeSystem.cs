using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using System;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(StepPhysicsWorldSystem)), UpdateBefore(typeof(EndFramePhysicsSystem))]
public partial class ExtractOnOverlapDamageOvertimeSystem : SimGameSystemBase
{
    private StepPhysicsWorldSystem _stepPhysicsWorldSystem;
    private PhysicsWorldSystem _physicsWorldSystem;
    private EndFramePhysicsSystem _endFramePhysicsSystem;
    private OnOverlapDamageOvertimeSystem _onOverlapDamageOvertimeSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _stepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorldSystem>();
        _physicsWorldSystem = World.GetOrCreateSystem<PhysicsWorldSystem>();
        _endFramePhysicsSystem = World.GetOrCreateSystem<EndFramePhysicsSystem>();
        _onOverlapDamageOvertimeSystem = World.GetOrCreateSystem<OnOverlapDamageOvertimeSystem>();
    }

    protected override void OnUpdate()
    {
        Dependency = new ExtractFromEventsJob()
        {
            OnOverlapDamageOvertimeSetting = GetComponentDataFromEntity<OnOverlapDamageOvertimeSetting>(isReadOnly: true),
            World = _physicsWorldSystem.PhysicsWorld,

            OutDamage = _onOverlapDamageOvertimeSystem.OutDamage

        }.Schedule(_stepPhysicsWorldSystem.Simulation, ref _physicsWorldSystem.PhysicsWorld, Dependency);

        _endFramePhysicsSystem.HandlesToWaitFor.Add(Dependency);
    }

    struct ExtractFromEventsJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<OnOverlapDamageOvertimeSetting> OnOverlapDamageOvertimeSetting;
        [ReadOnly] public PhysicsWorld World;

        public NativeList<(Entity instigator, Entity target, int damage)> OutDamage;

        public void Execute(TriggerEvent triggerEvent)
        {
            ProcessEntityPair(triggerEvent.EntityA, triggerEvent.EntityB, ref triggerEvent);
            ProcessEntityPair(triggerEvent.EntityB, triggerEvent.EntityA, ref triggerEvent);
        }

        private void ProcessEntityPair(Entity entityA, Entity entityB, ref TriggerEvent triggerEvent)
        {
            if (OnOverlapDamageOvertimeSetting.TryGetComponent(entityA, out OnOverlapDamageOvertimeSetting onOverlapDamageOvertimeSetting))
            {
                OutDamage.Add((entityA, entityB, onOverlapDamageOvertimeSetting.Damage));
            }
        }
    }
}

[UpdateInGroup(typeof(PostPhysicsSystemGroup))]
[AlwaysUpdateSystem]
public partial class OnOverlapDamageOvertimeSystem : SimGameSystemBase
{
    public NativeList<(Entity instigator, Entity target, int damage)> OutDamage;

    protected override void OnCreate()
    {
        base.OnCreate();
        OutDamage = new NativeList<(Entity, Entity, int)>(Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        OutDamage.Dispose();
    }

    protected override void OnUpdate()
    {
        // damage
        foreach ((Entity instigator, Entity target, int damage) in OutDamage)
        {
            if (EntityManager.TryGetComponent(instigator, out OnOverlapDamageOvertimeSetting onOverlapDamageOvertimeSetting))
            {
                if(EntityManager.TryGetBuffer(instigator, out DynamicBuffer<OnOverlapDamageOvertimeDamagedEntities> entitiesBuffer))
                {
                    var deltaTime = GetDeltaTime(onOverlapDamageOvertimeSetting.Delay.Type);

                    if (deltaTime > TimeValue.Zero)
                    {
                        if (EntityManager.TryGetComponent(instigator, out OnOverlapDamageOvertimeState onOverlapDamageOvertimeState))
                        {
                            if ((GetElapsedTime(onOverlapDamageOvertimeState.TrackedTime.Type) - onOverlapDamageOvertimeState.TrackedTime) >= onOverlapDamageOvertimeSetting.Delay)
                            {
                                entitiesBuffer.Clear();

                                EntityManager.SetComponentData(instigator, new OnOverlapDamageOvertimeState() { TrackedTime = GetElapsedTime(onOverlapDamageOvertimeState.TrackedTime.Type) });
                            }
                        }
                    }

                    NativeArray<OnOverlapDamageOvertimeDamagedEntities> entitiesArray = entitiesBuffer.ToNativeArray(Allocator.Temp);

                    bool entityHasAlreadyBeenHandled = false;
                    foreach (OnOverlapDamageOvertimeDamagedEntities entity in entitiesArray)
                    {
                        if (entity.Value == target)
                        {
                            entityHasAlreadyBeenHandled = true;
                            break;
                        }
                    }

                    if (!entityHasAlreadyBeenHandled)
                    {
                        entitiesBuffer.Add(new OnOverlapDamageOvertimeDamagedEntities() { Value = target });

                        DamageRequestSettings damageRequest = new DamageRequestSettings()
                        {
                            DamageAmount = damage,
                            InstigatorSet = CommonReads.GetInstigatorSetFromLastPhysicalInstigator(Accessor, instigator),
                            IsAutoAttack = false,
                        };
                        CommonWrites.RequestDamage(Accessor, damageRequest, target);
                    }
                }
            }
        }

        OutDamage.Clear();
    }
}