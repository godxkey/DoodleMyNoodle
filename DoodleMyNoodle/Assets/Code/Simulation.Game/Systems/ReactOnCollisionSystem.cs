using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(StepPhysicsWorldSystem)), UpdateBefore(typeof(EndFramePhysicsSystem))]
public class ExtractCollisionReactionsSystem : SimSystemBase
{
    private StepPhysicsWorldSystem _stepPhysicsWorldSystem;
    private PhysicsWorldSystem _physicsWorldSystem;
    private EndFramePhysicsSystem _endFramePhysicsSystem;
    private ReactOnCollisionSystem _reactSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _stepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorldSystem>();
        _physicsWorldSystem = World.GetOrCreateSystem<PhysicsWorldSystem>();
        _endFramePhysicsSystem = World.GetOrCreateSystem<EndFramePhysicsSystem>();
        _reactSystem = World.GetOrCreateSystem<ReactOnCollisionSystem>();
    }

    protected override void OnUpdate()
    {
        var job = new ExtractFromEventsJob()
        {
            Healths = GetComponentDataFromEntity<Health>(isReadOnly: true),
            DamageOnContacts = GetComponentDataFromEntity<DamageOnContact>(isReadOnly: true),
            ExplodeOnContacts = GetComponentDataFromEntity<ExplodeOnContact>(isReadOnly: true),
            DestroyOnContacts = GetComponentDataFromEntity<DestroyOnCollisionTag>(isReadOnly: true),
            HookDatas = GetComponentDataFromEntity<HookData>(isReadOnly: true),

            OutHooks = _reactSystem.HookRequests,
            OutDamages = _reactSystem.DamagesRequests,
            OutDestroys = _reactSystem.DestroyRequests,
            OutExplosions = _reactSystem.ExplosionRequests,
            World = _physicsWorldSystem.PhysicsWorld,

        }.Schedule(_stepPhysicsWorldSystem.Simulation, ref _physicsWorldSystem.PhysicsWorld, Dependency);

        _endFramePhysicsSystem.HandlesToWaitFor.Add(job);
    }

    struct ExtractFromEventsJob : ICollisionEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<Health> Healths;
        [ReadOnly] public ComponentDataFromEntity<DamageOnContact> DamageOnContacts;
        [ReadOnly] public ComponentDataFromEntity<ExplodeOnContact> ExplodeOnContacts;
        [ReadOnly] public ComponentDataFromEntity<DestroyOnCollisionTag> DestroyOnContacts;
        [ReadOnly] public ComponentDataFromEntity<HookData> HookDatas;
        [ReadOnly] public PhysicsWorld World;

        public NativeList<(Entity instigator, Entity target, int damage)> OutDamages;
        public NativeList<(Entity hook, Entity other)> OutHooks;
        public NativeList<Entity> OutDestroys;
        public NativeList<(Entity instigator, fix2 pos, fix radius, int damage)> OutExplosions;

        public void Execute(CollisionEvent collisionEvent)
        {
            ProcessEntityPair(collisionEvent.EntityA, collisionEvent.EntityB, ref collisionEvent);
            ProcessEntityPair(collisionEvent.EntityB, collisionEvent.EntityA, ref collisionEvent);
        }

        private void ProcessEntityPair(Entity entityA, Entity entityB, ref CollisionEvent collisionEvent)
        {
            if (DamageOnContacts.TryGetComponent(entityA, out DamageOnContact damageOnContact))
            {
                if (Healths.HasComponent(entityB))
                {
                    OutDamages.Add((entityA, entityB, damageOnContact.Value));
                }

                if (damageOnContact.DestroySelf)
                {
                    OutDestroys.AddUnique(entityA);
                }
            }

            if (DestroyOnContacts.HasComponent(entityA))
            {
                OutDestroys.AddUnique(entityA);
            }

            if (HookDatas.HasComponent(entityA))
            {
                OutHooks.AddUnique((entityA, entityB));
            }

            if (ExplodeOnContacts.TryGetComponent(entityA, out ExplodeOnContact explodeOnContact))
            {
                var details = collisionEvent.CalculateDetails(ref World);

                OutExplosions.Add((
                    instigator: entityA,
                    pos: details.AverageContactPointPositionFix,
                    radius: explodeOnContact.Radius,
                    damage: explodeOnContact.Damage));
            }
        }
    }
}

[UpdateInGroup(typeof(PostPhysicsSystemGroup))]
[AlwaysUpdateSystem]
public class ReactOnCollisionSystem : SimSystemBase
{
    public NativeList<(Entity instigator, Entity target, int damage)> DamagesRequests;
    public NativeList<Entity> DestroyRequests;
    public NativeList<(Entity instigator, fix2 pos, fix range, int damage)> ExplosionRequests;
    public NativeList<(Entity hook, Entity other)> HookRequests;

    protected override void OnCreate()
    {
        base.OnCreate();
        DamagesRequests = new NativeList<(Entity, Entity, int)>(Allocator.Persistent);
        DestroyRequests = new NativeList<Entity>(Allocator.Persistent);
        ExplosionRequests = new NativeList<(Entity, fix2, fix, int)>(Allocator.Persistent);
        HookRequests = new NativeList<(Entity, Entity)>(Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        DamagesRequests.Dispose();
        DestroyRequests.Dispose();
        ExplosionRequests.Dispose();
    }

    protected override void OnUpdate()
    {
        // damage
        foreach ((Entity instigator, Entity target, int damage) in DamagesRequests)
        {
            CommonWrites.RequestDamage(Accessor, instigator, target, damage);
        }

        // explosions
        foreach ((Entity instigator, fix2 pos, fix radius, int damage) in ExplosionRequests)
        {
            CommonWrites.RequestExplosion(Accessor, instigator, pos, radius, damage, true);
        }

        foreach (var item in HookRequests)
        {
            GetSingletonBuffer<SystemRequestHookContact>().Add(new SystemRequestHookContact()
            {
                HookEntity = item.hook,
                ContactEntity = item.other,
            });
        }

        // destroy
        EntityManager.DestroyEntity(DestroyRequests);

        DamagesRequests.Clear();
        DestroyRequests.Clear();
        ExplosionRequests.Clear();
        HookRequests.Clear();
    }
}
