using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;
using UnityEngineX;

public struct FallDamageBlacklistedEntity : ISingletonBufferElementData
{
    public Entity Entity;
    public fix BlacklistTime;
}

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
        Dependency = new ExtractFromEventsJob()
        {
            Healths = GetComponentDataFromEntity<Health>(isReadOnly: true),
            Translation = GetComponentDataFromEntity<FixTranslation>(isReadOnly: true),
            DamageOnContacts = GetComponentDataFromEntity<DamageOnContact>(isReadOnly: true),
            ExplodeOnContacts = GetComponentDataFromEntity<ExplodeOnContact>(isReadOnly: true),
            DestroyOnContacts = GetComponentDataFromEntity<DestroyOnCollisionTag>(isReadOnly: true),
            ImpulseOnContacts = GetComponentDataFromEntity<ImpulseOnContact>(isReadOnly: true),
            StickOnCollision = GetComponentDataFromEntity<StickOnCollisionTag>(isReadOnly: true),
            HookDatas = GetComponentDataFromEntity<HookData>(isReadOnly: true),
            LassoDatas = GetComponentDataFromEntity<LassoData>(isReadOnly: true),

            OutHooks = _reactSystem.HookRequests,
            OutLassos = _reactSystem.LassoRequests,
            OutDamages = _reactSystem.DamagesRequests,
            OutDestroys = _reactSystem.DestroyRequests,
            OutStick = _reactSystem.StickRequest,
            OutImpulse = _reactSystem.ImpulseRequest,
            OutExplosions = _reactSystem.ExplosionRequests,
            OutFallDamage = _reactSystem.FallDamageRequests,
            World = _physicsWorldSystem.PhysicsWorld,

        }.Schedule(_stepPhysicsWorldSystem.Simulation, ref _physicsWorldSystem.PhysicsWorld, Dependency);

        _endFramePhysicsSystem.HandlesToWaitFor.Add(Dependency);
    }

    struct ExtractFromEventsJob : ICollisionEventsJob
    {
        [ReadOnly] public ComponentDataFromEntity<Health> Healths;
        [ReadOnly] public ComponentDataFromEntity<FixTranslation> Translation;
        [ReadOnly] public ComponentDataFromEntity<DamageOnContact> DamageOnContacts;
        [ReadOnly] public ComponentDataFromEntity<ExplodeOnContact> ExplodeOnContacts;
        [ReadOnly] public ComponentDataFromEntity<DestroyOnCollisionTag> DestroyOnContacts;
        [ReadOnly] public ComponentDataFromEntity<ImpulseOnContact> ImpulseOnContacts;
        [ReadOnly] public ComponentDataFromEntity<StickOnCollisionTag> StickOnCollision;
        [ReadOnly] public ComponentDataFromEntity<HookData> HookDatas;
        [ReadOnly] public ComponentDataFromEntity<LassoData> LassoDatas;
        [ReadOnly] public PhysicsWorld World;

        public NativeList<(Entity instigator, Entity target, int damage)> OutDamages;
        public NativeList<(Entity hook, Entity other)> OutHooks;
        public NativeList<(Entity hook, Entity other, LassoData lasso)> OutLassos;
        public NativeList<Entity> OutDestroys;
        public NativeList<Entity> OutStick;
        public NativeList<(Entity other, fix2 strength)> OutImpulse;
        public NativeList<(Entity instigator, fix2 pos, fix radius, int damage)> OutExplosions;
        public NativeList<(Entity instigator, Entity target, fix impulse)> OutFallDamage;

        public void Execute(CollisionEvent collisionEvent)
        {
            ProcessEntityPair(collisionEvent.EntityA, collisionEvent.EntityB, ref collisionEvent);
            ProcessEntityPair(collisionEvent.EntityB, collisionEvent.EntityA, ref collisionEvent);

            if (collisionEvent.VelocityA.Linear.lengthSquared >= 1 || collisionEvent.VelocityB.Linear.lengthSquared >= 1)
            {
                var details = collisionEvent.CalculateDetails(ref World);

                if (details.EstimatedImpulse > 1)
                {
                    OutFallDamage.Add((
                        instigator: collisionEvent.EntityA,
                        target: collisionEvent.EntityB,
                        impulse: (fix)details.EstimatedImpulse));

                    OutFallDamage.Add((
                        instigator: collisionEvent.EntityB,
                        target: collisionEvent.EntityA,
                        impulse: (fix)details.EstimatedImpulse));
                }

            }
        }

        private void ProcessEntityPair(Entity entityA, Entity entityB, ref CollisionEvent collisionEvent)
        {
            if (DamageOnContacts.TryGetComponent(entityA, out DamageOnContact damageOnContact))
            {
                OutDamages.Add((entityA, entityB, damageOnContact.Value));

                if (damageOnContact.DestroySelf)
                {
                    OutDestroys.AddUnique(entityA);
                }
            }

            if (ImpulseOnContacts.TryGetComponent(entityA, out ImpulseOnContact impulseOnContact))
            {
                if (Translation.TryGetComponent(entityA, out FixTranslation fixTranslationA)
                    && Translation.TryGetComponent(entityB, out FixTranslation fixTranslationB))
                {
                    fix2 direction = fixTranslationB.Value - fixTranslationA.Value;
                    direction.Normalize();

                    OutImpulse.AddUnique((entityB, direction * impulseOnContact.Strength));
                }
            }

            if (StickOnCollision.HasComponent(entityA))
            {
                OutStick.AddUnique(entityA);
            }

            if (DestroyOnContacts.HasComponent(entityA))
            {
                OutDestroys.AddUnique(entityA);
            }

            if (HookDatas.HasComponent(entityA))
            {
                OutHooks.AddUnique((entityA, entityB));
            }

            if (LassoDatas.HasComponent(entityA) && LassoDatas.TryGetComponent(entityA, out LassoData lassoData))
            {
                OutLassos.AddUnique((entityA, entityB, lassoData));
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
public class ReactOnCollisionSystem : SimSystemBase
{
    public static LogChannel FallDamageLogChannel = Log.CreateChannel("Fall Damage", activeByDefault: false);

    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;

    public NativeList<(Entity instigator, Entity target, int damage)> DamagesRequests;
    public NativeList<Entity> DestroyRequests;
    public NativeList<Entity> StickRequest;
    public NativeList<(Entity other, fix2 strength)> ImpulseRequest;
    public NativeList<(Entity instigator, fix2 pos, fix range, int damage)> ExplosionRequests;
    public NativeList<(Entity hook, Entity other)> HookRequests;
    public NativeList<(Entity lasso, Entity other, LassoData)> LassoRequests;
    public NativeList<(Entity instigator, Entity target, fix impulse)> FallDamageRequests;

    protected override void OnCreate()
    {
        base.OnCreate();
        DamagesRequests = new NativeList<(Entity, Entity, int)>(Allocator.Persistent);
        DestroyRequests = new NativeList<Entity>(Allocator.Persistent);
        StickRequest = new NativeList<Entity>(Allocator.Persistent);
        ImpulseRequest = new NativeList<(Entity, fix2)>(Allocator.Persistent);
        ExplosionRequests = new NativeList<(Entity, fix2, fix, int)>(Allocator.Persistent);
        HookRequests = new NativeList<(Entity, Entity)>(Allocator.Persistent);
        LassoRequests = new NativeList<(Entity, Entity, LassoData)>(Allocator.Persistent);
        FallDamageRequests = new NativeList<(Entity, Entity, fix)>(Allocator.Persistent);

        m_EndSimulationEcbSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();

       RequireSingletonForUpdate<GridInfo>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        DamagesRequests.Dispose();
        DestroyRequests.Dispose();
        StickRequest.Dispose();
        ImpulseRequest.Dispose();
        ExplosionRequests.Dispose();
        HookRequests.Dispose();
        LassoRequests.Dispose();
        FallDamageRequests.Dispose();
    }

    protected override void OnUpdate()
    {
        // damage
        foreach ((Entity instigator, Entity target, int damage) in DamagesRequests)
        {
            if (TryGetComponent(instigator, out EffectGroupComponent effectGroupComponent))
            {
                CommonWrites.RequestDamage(Accessor, target, damage, effectGroupComponent.ID);
            }
            else
            {
                CommonWrites.RequestDamage(Accessor, target, damage);
            }
        }

        // impulse
        foreach ((Entity other, fix2 strength) in ImpulseRequest)
        {
            CommonWrites.RequestImpulse(Accessor, other, strength);
        }

        // explosions
        foreach ((Entity instigator, fix2 pos, fix radius, int damage) in ExplosionRequests)
        {
            CommonWrites.RequestExplosion(Accessor, instigator, pos, radius, damage, true);
        }

        HandleFallDamage();

        // Stick
        foreach (Entity entity in StickRequest)
        {
            if (EntityManager.HasComponent<PhysicsVelocity>(entity))
            {
                EntityManager.SetComponentData(entity, new PhysicsVelocity() { Linear = new fix2() });
                EntityManager.SetComponentData(entity, new StickOnCollisionTag() { Sticked = true });
            }
        }

        foreach (var item in HookRequests)
        {
            GetSingletonBuffer<SystemRequestHookContact>().Add(new SystemRequestHookContact()
            {
                HookEntity = item.hook,
                ContactEntity = item.other,
            });
        }

        foreach ((Entity instigator, Entity target, LassoData lasso) in LassoRequests)
        {
            if (EntityManager.TryGetComponentData(instigator, out ProjectileInstigator projInstigator))
            {
                if (EntityManager.HasComponent<PhysicsVelocity>(target) && EntityManager.TryGetComponentData(projInstigator.Value, out FixTranslation translation))
                {
                    CommonWrites.RequestPull(Accessor, target, translation.Value, lasso.PullSpeed);
                }
                else
                {
                    if (EntityManager.HasComponent<PhysicsVelocity>(projInstigator.Value) && EntityManager.TryGetComponentData(instigator, out FixTranslation instTranslation))
                    {
                        fix2 position = instTranslation + new fix2(0, (fix)0.5);
                        CommonWrites.RequestPull(Accessor, projInstigator.Value, position, lasso.PullSpeed);
                    }
                }
            }
        }

        // destroy
        foreach (var item in DestroyRequests)
        {
            m_EndSimulationEcbSystem.CreateCommandBuffer().DestroyEntity(item);
        }

        DamagesRequests.Clear();
        DestroyRequests.Clear();
        StickRequest.Clear();
        ExplosionRequests.Clear();
        HookRequests.Clear();
        LassoRequests.Clear();
    }

    private void HandleFallDamage()
    {
        var blacklistedEntities = GetSingletonBuffer<FallDamageBlacklistedEntity>();
        var tileWorld = CommonReads.GetTileWorld(Accessor);
        for (int i = 0; i < FallDamageRequests.Length; i++)
        {
            (Entity instigator, Entity target, fix impulse) = FallDamageRequests[i];

            // break if entity blacklisted
            bool isBlacklisted = false;
            for (int b = 0; b < blacklistedEntities.Length; b++)
            {
                if (blacklistedEntities[b].Entity == target)
                {
                    isBlacklisted = true;
                    break;
                }
            }

            if (isBlacklisted)
                break;

            bool breakTile = false;
            bool damage = false;
            fix damageImpulseThreshold = 0;

            if (HasComponent<TileColliderTag>(target)
                && impulse > SimulationGameConstants.ImpulseThresholdDestroyingTile)
            {
                int2 tile = Helpers.GetTile(GetComponent<FixTranslation>(target));
                var tileFlags = tileWorld.GetFlags(tile);
                if (tileFlags.IsDestructible)
                {
                    CommonWrites.RequestTransformTile(Accessor, tile, TileFlagComponent.Empty);
                    breakTile = true;
                }
            }

            if (HasComponent<Health>(target))
            {
                damageImpulseThreshold = SimulationGameConstants.ImpulseThresholdFallDamageNonNavAgents;
                if (TryGetComponent(target, out NavAgentFootingState footing))
                {
                    damageImpulseThreshold = footing.Value == NavAgentFooting.None ? SimulationGameConstants.ImpulseThresholdFallDamageNoAirControl : SimulationGameConstants.ImpulseThresholdFallDamageAirControl;
                }

                if (impulse > damageImpulseThreshold)
                {
                    CommonWrites.RequestDamage(Accessor, target, SimulationGameConstants.FallDamage);
                    damage = true;
                    blacklistedEntities.Add(new FallDamageBlacklistedEntity() { Entity = target, BlacklistTime = Time.ElapsedTime });
                }
            }

            if (FallDamageLogChannel.Active)
            {
                Log.Info(FallDamageLogChannel, $"Falldamage:" +
                    $"   time={Time.ElapsedTime}" +
                    $"   target={EntityManager.GetNameSafe(target)}" +
                    $"   instigator={EntityManager.GetNameSafe(instigator)}" +
                    $"   impulse={impulse}" +
                    $"   breakTile={breakTile}" +
                    $"   damage={damage}" +
                    $"   damageImpulseThreshold={damageImpulseThreshold}");
            }
        }

        // remove outdated blacklisted entities
        for (int i = blacklistedEntities.Length - 1; i >= 0; i--)
        {
            if (Time.ElapsedTime - blacklistedEntities[i].BlacklistTime >= SimulationGameConstants.FallDamageCooldown)
            {
                blacklistedEntities.RemoveAt(i);
            }
        }
        FallDamageRequests.Clear();
    }
}
