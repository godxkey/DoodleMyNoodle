using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using System;
using UnityEngine.Profiling;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(StepPhysicsWorldSystem)), UpdateBefore(typeof(EndFramePhysicsSystem))]
public class ExtractSignalEmitterOverlapsSystem : SimSystemBase
{
    private StepPhysicsWorldSystem _stepPhysicsWorldSystem;
    private PhysicsWorldSystem _physicsWorldSystem;
    private EndFramePhysicsSystem _endFramePhysicsSystem;
    private SetSignalSystem _setSignalSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _stepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorldSystem>();
        _physicsWorldSystem = World.GetOrCreateSystem<PhysicsWorldSystem>();
        _endFramePhysicsSystem = World.GetOrCreateSystem<EndFramePhysicsSystem>();
        _setSignalSystem = World.GetOrCreateSystem<SetSignalSystem>();
    }

    protected override void OnUpdate()
    {
        var job = new ExtractFromEventsJob()
        {
            Signals = GetComponentDataFromEntity<Signal>(isReadOnly: true),
            SignalEmissionTypes = GetComponentDataFromEntity<SignalEmissionType>(isReadOnly: true),
            SignalEmissionStatuses = GetComponentDataFromEntity<SignalEmissionFlags>(isReadOnly: true),
            OutOverlappingEmitters = _setSignalSystem.EmitterOverlaps,
            World = _physicsWorldSystem.PhysicsWorld,

        }.Schedule(_stepPhysicsWorldSystem.Simulation, ref _physicsWorldSystem.PhysicsWorld, Dependency);

        _endFramePhysicsSystem.HandlesToWaitFor.Add(job);
        _setSignalSystem.HandlesToWaitFor.Add(job);
    }

    struct ExtractFromEventsJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<Signal> Signals;
        [ReadOnly] public ComponentDataFromEntity<SignalEmissionType> SignalEmissionTypes;
        [ReadOnly] public ComponentDataFromEntity<SignalEmissionFlags> SignalEmissionStatuses;
        [ReadOnly] public PhysicsWorld World;

        public NativeList<Entity> OutOverlappingEmitters;

        public void Execute(TriggerEvent triggerEvent)
        {
            ProcessEntityPair(triggerEvent.EntityA, triggerEvent.EntityB, ref triggerEvent);
            ProcessEntityPair(triggerEvent.EntityB, triggerEvent.EntityA, ref triggerEvent);
        }

        private void ProcessEntityPair(Entity entityA, Entity entityB, ref TriggerEvent collisionEvent)
        {
            if (!Signals.HasComponent(entityA))
                return;

            ESignalEmissionType type = SignalEmissionTypes[entityA].Value;

            if (type == ESignalEmissionType.WhileOverlap || type == ESignalEmissionType.OnEnter)
            {
                OutOverlappingEmitters.Add(entityA);
            }
        }
    }
}

[UpdateInGroup(typeof(SignalSystemGroup))]
public class ResetSignalSystems : SimSystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref Signal signalEmission, in SignalStayOnForever stayOnForever, in PreviousSignal previousSignal) =>
        {
            signalEmission.Value = stayOnForever && previousSignal.Value;
        }).Schedule();
    }
}

[UpdateAfter(typeof(ResetSignalSystems))]
[UpdateInGroup(typeof(SignalSystemGroup))]
[AlwaysUpdateSystem]
public class SetSignalSystem : SimSystemBase
{
    public NativeList<Entity> EmitterClickRequests;
    public NativeList<JobHandle> HandlesToWaitFor;
    public NativeList<Entity> EmitterOverlaps;

    protected override void OnCreate()
    {
        base.OnCreate();
        EmitterClickRequests = new NativeList<Entity>(Allocator.Persistent);
        EmitterOverlaps = new NativeList<Entity>(Allocator.Persistent);
        HandlesToWaitFor = new NativeList<JobHandle>(Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EmitterClickRequests.Dispose();
        EmitterOverlaps.Dispose();
        HandlesToWaitFor.Dispose();
    }

    protected override void OnUpdate()
    {
        JobHandle.CombineDependencies(HandlesToWaitFor.AsArray()).Complete();
        HandlesToWaitFor.Clear();

        SetAlwaysOnEmissions();
        SetClickEmissions();
        SetOverlapEmissions();

        // should be placed last for better responsivity
        SetLogicEmissions();
    }

    private void SetAlwaysOnEmissions()
    {
        // set all signals ON for emitters with ToggleClick ON
        Entities.ForEach((ref Signal signal, in SignalEmissionType emission) =>
        {
            if (emission.Value == ESignalEmissionType.AlwaysOn)
            {
                signal = true;
            }
        }).Schedule();
    }

    private void SetClickEmissions()
    {
        var clickRequests = EmitterClickRequests;
        Job.WithCode(() =>
        {
            for (int i = 0; i < clickRequests.Length; i++)
            {
                Entity emitter = clickRequests[i];

                // NB: We need to check if entity exists here since 'EmitterClickRequests' comes from external systems we don't trust
                if (!HasComponent<SignalEmissionType>(emitter))
                    continue;

                ESignalEmissionType type = GetComponent<SignalEmissionType>(emitter).Value;

                if (type == ESignalEmissionType.OnClick)
                {
                    SetComponent<Signal>(emitter, true);
                }
                else if (type == ESignalEmissionType.ToggleOnClick)
                {
                    var status = GetComponent<SignalEmissionFlags>(emitter);
                    status.ToggleClickOn = !status.ToggleClickOn;
                    SetComponent(emitter, status);
                }
            }
            clickRequests.Clear();
        }).Schedule();

        // set all signals ON for emitters with ToggleClick ON
        Entities.ForEach((ref Signal signal, in SignalEmissionFlags flags) =>
        {
            if (flags.ToggleClickOn)
            {
                signal = true;
            }
        }).Schedule();
    }

    private void SetOverlapEmissions()
    {
        // reset 'Overlapping' flags
        var overlaps = EmitterOverlaps;
        Entities.ForEach((Entity emitter, ref SignalEmissionFlags flags) =>
        {
            if (flags.Overlapping && !overlaps.Contains(emitter))
            {
                flags.Overlapping = false;
            }
        }).Schedule();

        Job.WithCode(() =>
        {
            for (int i = 0; i < overlaps.Length; i++)
            {
                Entity emitter = overlaps[i];

                // NB: No need to check if entity exists here since 'EmitterOverlaps' comes from our own system we trust.
                var type = GetComponent<SignalEmissionType>(emitter).Value;
                var status = GetComponent<SignalEmissionFlags>(emitter);

                // set emission (if type is OnEnter, only set it if we were NOT overlapping last frame)
                if (type != ESignalEmissionType.OnEnter || !status.Overlapping)
                {
                    SetComponent<Signal>(emitter, true);
                }

                // update status
                status.Overlapping = true;
                SetComponent(emitter, status);
            }
            overlaps.Clear();
        }).Schedule();
    }

    private void SetLogicEmissions()
    {
        Entities.ForEach((Entity emitter, DynamicBuffer<SignalLogicTarget> logicTargets, in SignalEmissionType type) =>
        {
            if (type.Value == ESignalEmissionType.AND)
            {
                bool on = logicTargets.Length > 0;
                foreach (var item in logicTargets)
                {
                    if (!GetComponent<Signal>(item))
                    {
                        on = false;
                        break;
                    }
                }

                SetComponent<Signal>(emitter, on);
            }
            else if (type.Value == ESignalEmissionType.OR)
            {
                bool on = false;
                foreach (var item in logicTargets)
                {
                    if (GetComponent<Signal>(item))
                    {
                        on = true;
                        break;
                    }
                }
                SetComponent<Signal>(emitter, on);
            }
        }).Schedule();
    }
}

//[UpdateInGroup(typeof(SignalSystemGroup))]
//[UpdateAfter(typeof(SetSignalSystem))]
//public class PropagateSignalSystem : SimSystemBase
//{
//    protected override void OnUpdate()
//    {
//        Entities
//            .WithAll<Signal>()
//            .ForEach((Entity signalEntity, DynamicBuffer<SignalPropagationTarget> targets) =>
//        {
//            if (!GetComponent<Signal>(signalEntity)) // skip disabled emitters
//                return;

//            for (int i = 0; i < targets.Length; i++)
//            {
//                if (HasComponent<Signal>(targets[i]))
//                {
//                    SetComponent<Signal>(targets[i], true);
//                }
//            }
//        }).Schedule();
//    }
//}

[UpdateInGroup(typeof(SignalSystemGroup))]
[UpdateAfter(typeof(SetSignalSystem))]
public class RecordPreviousSignalsSystem : SimSystemBase
{
    protected override void OnUpdate()
    {
        Entities.ForEach((ref PreviousSignal previousSignal, in Signal signal) =>
        {
            previousSignal.Value = signal;
        }).Schedule();
    }
}