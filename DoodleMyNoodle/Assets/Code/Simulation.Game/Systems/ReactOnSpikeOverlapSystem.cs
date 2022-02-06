using Unity.Entities;
using Unity.Mathematics;
using CCC.Fix2D;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Jobs;
using System;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(StepPhysicsWorldSystem)), UpdateBefore(typeof(EndFramePhysicsSystem))]
public class ExtractFromSpikeOverlapSystem : SimGameSystemBase
{
    private StepPhysicsWorldSystem _stepPhysicsWorldSystem;
    private PhysicsWorldSystem _physicsWorldSystem;
    private EndFramePhysicsSystem _endFramePhysicsSystem;
    private ReactOnSpikeOverlapSystem _reactOnSpikeOverlapSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _stepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorldSystem>();
        _physicsWorldSystem = World.GetOrCreateSystem<PhysicsWorldSystem>();
        _endFramePhysicsSystem = World.GetOrCreateSystem<EndFramePhysicsSystem>();
        _reactOnSpikeOverlapSystem = World.GetOrCreateSystem<ReactOnSpikeOverlapSystem>();
    }

    protected override void OnUpdate()
    {
        Dependency = new ExtractFromEventsJob()
        {
            SpikeTags = GetComponentDataFromEntity<SpikeTrap>(isReadOnly: true),
            DamageOnContacts = GetComponentDataFromEntity<DamageOnContact>(isReadOnly: true),
            Healths = GetComponentDataFromEntity<Health>(isReadOnly: true),
            FixTranslations = GetComponentDataFromEntity<FixTranslation>(isReadOnly: true),
            PhysicsVelocity = GetComponentDataFromEntity<PhysicsVelocity>(isReadOnly: true),
            World = _physicsWorldSystem.PhysicsWorld,

            OutSpikeDamages = _reactOnSpikeOverlapSystem.OutSpikeDamages,
            OutSpikeDestroys = _reactOnSpikeOverlapSystem.OutSpikeDestroys,
            CurrentSpikeCooldowns = _reactOnSpikeOverlapSystem.SpikeCooldowns,

        }.Schedule(_stepPhysicsWorldSystem.Simulation, ref _physicsWorldSystem.PhysicsWorld, Dependency);

        _endFramePhysicsSystem.HandlesToWaitFor.Add(Dependency);
    }

    struct ExtractFromEventsJob : ITriggerEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<SpikeTrap> SpikeTags;
        [ReadOnly] public ComponentDataFromEntity<DamageOnContact> DamageOnContacts;
        [ReadOnly] public ComponentDataFromEntity<Health> Healths;
        [ReadOnly] public ComponentDataFromEntity<FixTranslation> FixTranslations;
        [ReadOnly] public ComponentDataFromEntity<PhysicsVelocity> PhysicsVelocity;
        [ReadOnly] public PhysicsWorld World;

        public NativeList<(Entity instigator, Entity target, int damage)> OutSpikeDamages;
        public NativeList<Entity> OutSpikeDestroys;
        public NativeList<SpikeDamageCooldown> CurrentSpikeCooldowns;

        public void Execute(TriggerEvent triggerEvent)
        {
            ProcessEntityPair(triggerEvent.EntityA, triggerEvent.EntityB, ref triggerEvent);
            ProcessEntityPair(triggerEvent.EntityB, triggerEvent.EntityA, ref triggerEvent);
        }

        private void ProcessEntityPair(Entity entityA, Entity entityB, ref TriggerEvent collisionEvent)
        {
            if (SpikeTags.TryGetComponent(entityA, out SpikeTrap spikeTrap) && DamageOnContacts.TryGetComponent(entityA, out DamageOnContact damageOnContact))
            {
                bool canGetSpiked = true;
                if (FixTranslations.TryGetComponent(entityA, out FixTranslation translationA) && FixTranslations.TryGetComponent(entityB, out FixTranslation translationB))
                {
                    fix2 deltaPos = translationB.Value - translationA.Value;
                    if (spikeTrap.DamageDirection.x > 0 && deltaPos.x < 0)
                    {
                        canGetSpiked = false;
                    }
                    else if (spikeTrap.DamageDirection.x < 0 && deltaPos.x > 0)
                    {
                        canGetSpiked = false;
                    }
                    else if (spikeTrap.DamageDirection.y > 0 && deltaPos.y < 0)
                    {
                        canGetSpiked = false;
                    }
                    else if (spikeTrap.DamageDirection.y < 0 && deltaPos.y > 0)
                    {
                        canGetSpiked = false;
                    }
                }

                if (PhysicsVelocity.TryGetComponent(entityB, out PhysicsVelocity velocity))
                {
                    if (spikeTrap.DamageDirection.x > 0 && velocity.Linear.x > 0)
                    {
                        canGetSpiked = false;
                    }
                    else if (spikeTrap.DamageDirection.x < 0 && velocity.Linear.x < 0)
                    {
                        canGetSpiked = false;
                    }
                    else if (spikeTrap.DamageDirection.y > 0 && velocity.Linear.y > 0)
                    {
                        canGetSpiked = false;
                    }
                    else if (spikeTrap.DamageDirection.y < 0 && velocity.Linear.y < 0)
                    {
                        canGetSpiked = false;
                    }
                    else if (spikeTrap.DamageDirection.x == 0 && velocity.Linear.y == 0)
                    {
                        canGetSpiked = false;
                    }
                    else if (spikeTrap.DamageDirection.y == 0 && velocity.Linear.x == 0)
                    {
                        canGetSpiked = false;
                    }
                }

                foreach (SpikeDamageCooldown spikeDamageCooldown in CurrentSpikeCooldowns)
                {
                    if (spikeDamageCooldown.EntityLinkedTo == entityB)
                    {
                        canGetSpiked = false;
                    }
                }

                if (canGetSpiked)
                {
                    if (Healths.HasComponent(entityB))
                    {
                        OutSpikeDamages.Add((entityA, entityB, damageOnContact.Value));
                    }

                    if (damageOnContact.DestroySelf)
                    {
                        OutSpikeDestroys.AddUnique(entityA);
                    }
                }
            }
        }
    }
}

[UpdateAfter(typeof(PhysicsSystemGroup))]
public class ResetSpikeCooldownSystems : SimGameSystemBase
{
    private NativeList<Entity> SpikeCooldownEntityToDestroy;
    private ReactOnSpikeOverlapSystem _reactOnSpikeOverlapSystem;

    protected override void OnCreate()
    {
        base.OnCreate();
        SpikeCooldownEntityToDestroy = new NativeList<Entity>(Allocator.Persistent);
        _reactOnSpikeOverlapSystem = World.GetOrCreateSystem<ReactOnSpikeOverlapSystem>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SpikeCooldownEntityToDestroy.Dispose();
    }

    protected override void OnUpdate()
    {
        NativeList<Entity> SpikeCooldownEntityToDestroy = this.SpikeCooldownEntityToDestroy;
        NativeList<SpikeDamageCooldown> SpikeCooldownToRemove = _reactOnSpikeOverlapSystem.SpikeCooldowns;
        fix ElapsedTime = Time.ElapsedTime;

        Entities.ForEach((Entity entity, in SpikeDamageCooldown spikeDamageCooldown) =>
        {
            if (ElapsedTime > (spikeDamageCooldown.ElapsedTimeWhenCreated + spikeDamageCooldown.Cooldown))
            {
                SpikeCooldownEntityToDestroy.Add(entity);

                for (int i = 0; i < SpikeCooldownToRemove.Length; i++)
                {
                    SpikeDamageCooldown spikeCooldown = SpikeCooldownToRemove[i];
                    if (spikeCooldown.EntityLinkedTo == spikeDamageCooldown.EntityLinkedTo)
                    {
                        SpikeCooldownToRemove.RemoveAt(i);
                        break;
                    }
                }
            }
        }).Run();

        EntityManager.DestroyEntity(SpikeCooldownEntityToDestroy);

        SpikeCooldownEntityToDestroy.Clear();
    }
}

[UpdateInGroup(typeof(PostPhysicsSystemGroup))]
[AlwaysUpdateSystem]
public class ReactOnSpikeOverlapSystem : SimGameSystemBase
{
    public NativeList<(Entity instigator, Entity target, int damage)> OutSpikeDamages;
    public NativeList<Entity> OutSpikeDestroys;
    public NativeList<SpikeDamageCooldown> SpikeCooldowns;

    public EntityArchetype SpikeCooldownArchetype { get; private set; }

    protected override void OnCreate()
    {
        base.OnCreate();
        OutSpikeDamages = new NativeList<(Entity, Entity, int)>(Allocator.Persistent);
        OutSpikeDestroys = new NativeList<Entity>(Allocator.Persistent);
        SpikeCooldowns = new NativeList<SpikeDamageCooldown>(Allocator.Persistent);

        SpikeCooldownArchetype = EntityManager.CreateArchetype(typeof(SpikeDamageCooldown));
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        OutSpikeDamages.Dispose();
        OutSpikeDestroys.Dispose();
        SpikeCooldowns.Dispose();
    }

    protected override void OnUpdate()
    {
        // damage
        foreach ((Entity instigator, Entity target, int damage) in OutSpikeDamages)
        {
            Entity newSpikeCooldownEntity = EntityManager.CreateEntity(SpikeCooldownArchetype);
            SetComponent(newSpikeCooldownEntity, new SpikeDamageCooldown() { Cooldown = 1, ElapsedTimeWhenCreated = Time.ElapsedTime, EntityLinkedTo = target });
            SpikeCooldowns.Add(EntityManager.GetComponentData<SpikeDamageCooldown>(newSpikeCooldownEntity));

            CommonWrites.RequestDamage(Accessor, target, damage);
        }

        // destroy
        EntityManager.DestroyEntity(OutSpikeDestroys);

        OutSpikeDamages.Clear();
        OutSpikeDestroys.Clear();
    }
}