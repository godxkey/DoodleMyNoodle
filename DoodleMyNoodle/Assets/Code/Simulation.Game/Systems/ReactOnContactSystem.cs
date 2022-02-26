using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;
using UnityEngineX;
using Unity.Jobs;


public struct MutedContactActionElement : ISingletonBufferElementData
{
    public Entity Instigator;
    public Entity Target;
    public byte ContactActionBufferId; // the id of the element within the buffer
    public fix ExpirationTime;
}

static class MutedContactActionElementExtensions
{
    public static bool IsMuted(this DynamicBuffer<MutedContactActionElement> buffer, Entity entity, Entity target, byte actionId)
    {
        for (int i = 0; i < buffer.Length; i++)
        {
            var data = buffer[i];
            if (data.Instigator == entity && data.Target == target && data.ContactActionBufferId == actionId)
                return true;
        }
        return false;
    }
}

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(StepPhysicsWorldSystem)), UpdateBefore(typeof(EndFramePhysicsSystem))]
public class ExtractCollisionReactionsSystem : SimGameSystemBase
{
    private StepPhysicsWorldSystem _stepPhysicsWorldSystem;
    private PhysicsWorldSystem _physicsWorldSystem;
    private EndFramePhysicsSystem _endFramePhysicsSystem;
    private ExecuteGameActionSystem _gameActionSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _stepPhysicsWorldSystem = World.GetOrCreateSystem<StepPhysicsWorldSystem>();
        _physicsWorldSystem = World.GetOrCreateSystem<PhysicsWorldSystem>();
        _endFramePhysicsSystem = World.GetOrCreateSystem<EndFramePhysicsSystem>();
        _gameActionSystem = World.GetOrCreateSystem<ExecuteGameActionSystem>();
    }

    protected override void OnUpdate()
    {
        ExtractFromCollisionOrTrigger jobProcessor = new ExtractFromCollisionOrTrigger()
        {
            World = _physicsWorldSystem.PhysicsWorld,
            OutActions = _gameActionSystem.CreateRequestBuffer(),
            ActionOnContacts = GetBufferFromEntity<ActionOnColliderContact>(isReadOnly: true),
            Teams = GetComponentDataFromEntity<Team>(isReadOnly: true),
            FirstInstigators = GetComponentDataFromEntity<FirstInstigator>(isReadOnly: true),
            TileColliderTags = GetComponentDataFromEntity<TileColliderTag>(isReadOnly: true),
            MutedActions = GetSingletonBuffer<MutedContactActionElement>(),
        };

        Dependency = new ExtractFromTriggerEventsJob()
        {
            Processor = jobProcessor
        }.Schedule(_stepPhysicsWorldSystem.Simulation, ref _physicsWorldSystem.PhysicsWorld, Dependency);

        Dependency = new ExtractFromCollisionEventsJob()
        {
            Processor = jobProcessor
        }.Schedule(_stepPhysicsWorldSystem.Simulation, ref _physicsWorldSystem.PhysicsWorld, Dependency);

        _endFramePhysicsSystem.HandlesToWaitFor.Add(Dependency);
    }

    struct ExtractFromTriggerEventsJob : ITriggerEventsJob
    {
        public ExtractFromCollisionOrTrigger Processor;

        public void Execute(TriggerEvent collisionEvent)
        {
            Processor.Execute(collisionEvent);
        }
    }

    struct ExtractFromCollisionEventsJob : ICollisionEventsJob
    {
        public ExtractFromCollisionOrTrigger Processor;

        public void Execute(CollisionEvent collisionEvent)
        {
            Processor.Execute(collisionEvent);
        }
    }

    struct ExtractFromCollisionOrTrigger
    {
        [ReadOnly] public BufferFromEntity<ActionOnColliderContact> ActionOnContacts;
        [ReadOnly] public ComponentDataFromEntity<Team> Teams;
        [ReadOnly] public ComponentDataFromEntity<FirstInstigator> FirstInstigators;
        [ReadOnly] public ComponentDataFromEntity<TileColliderTag> TileColliderTags;
        [ReadOnly] public PhysicsWorld World;

        public DynamicBuffer<MutedContactActionElement> MutedActions;
        public NativeList<GameActionRequest> OutActions;

        private struct EntityInfo
        {
            public Team Team;
            public bool IsTerrain;
        }

        public void Execute(CollisionEvent collisionEvent)
        {
            ProcessEntityPair(collisionEvent.EntityA, collisionEvent.EntityB);
            ProcessEntityPair(collisionEvent.EntityB, collisionEvent.EntityA);
        }

        public void Execute(TriggerEvent triggerEvent)
        {
            ProcessEntityPair(triggerEvent.EntityA, triggerEvent.EntityB);
            ProcessEntityPair(triggerEvent.EntityB, triggerEvent.EntityA);
        }

        private void ProcessEntityPair(Entity entityA, Entity entityB)
        {
            if (ActionOnContacts.HasComponent(entityA))
            {
                DynamicBuffer<ActionOnColliderContact> actions = ActionOnContacts[entityA];
                ActorFilterInfo entityAInfo = Helpers.GetActorFilterInfo(entityA, Teams, FirstInstigators, TileColliderTags);
                ActorFilterInfo entityBInfo = Helpers.GetActorFilterInfo(entityB, Teams, FirstInstigators, TileColliderTags);

                for (int i = 0; i < actions.Length; i++)
                {
                    ActionOnColliderContact actionOnContact = actions[i];

                    if (MutedActions.IsMuted(entityA, entityB, actionOnContact.Data.Id))
                        continue;

                    if (Helpers.ActorFilterMatches(entityAInfo, entityBInfo, actionOnContact.Data.ActionFilter))
                    {
                        OutActions.Add(new GameActionRequest()
                        {
                            Instigator = entityA,
                            Target = entityB,
                            ActionEntity = actionOnContact.Data.ActionEntity,
                        });

                        if (actionOnContact.Data.SameTargetCooldown > 0)
                        {
                            MutedActions.Add(new MutedContactActionElement()
                            {
                                Instigator = entityA,
                                ContactActionBufferId = actionOnContact.Data.Id,
                                ExpirationTime = actionOnContact.Data.SameTargetCooldown,
                                Target = entityB
                            });
                        }
                    }
                }
            }
        }
    }
}

[UpdateInGroup(typeof(PostPhysicsSystemGroup))]
public class ExtractOverlapReactionsSystem : SimGameSystemBase
{
    private PhysicsWorldSystem _physicsWorldSystem;
    private ExecuteGameActionSystem _gameActionSystem;

    protected override void OnCreate()
    {
        base.OnCreate();

        _physicsWorldSystem = World.GetOrCreateSystem<PhysicsWorldSystem>();
        _gameActionSystem = World.GetOrCreateSystem<ExecuteGameActionSystem>();
    }

    protected override void OnUpdate()
    {
        var physicsWorld = _physicsWorldSystem.PhysicsWorld;
        var physicsBodiesMap = _physicsWorldSystem.EntityToPhysicsBody;
        var hits = new NativeList<DistanceHit>(Allocator.TempJob);
        var outActions = _gameActionSystem.CreateRequestBuffer();
        var teams = GetComponentDataFromEntity<Team>(isReadOnly: true);
        var firstInstigators = GetComponentDataFromEntity<FirstInstigator>(isReadOnly: true);
        var tileColliderTags = GetComponentDataFromEntity<TileColliderTag>(isReadOnly: true);
        var mutedActions = GetSingletonBuffer<MutedContactActionElement>();

        Entities
            .WithReadOnly(physicsWorld)
            .WithReadOnly(physicsBodiesMap)
            .WithReadOnly(teams)
            .WithReadOnly(firstInstigators)
            .WithReadOnly(tileColliderTags)
            .ForEach((Entity entity, DynamicBuffer<ActionOnOverlap> actionsOnOverlap, in FixTranslation position) =>
        {
            bool ignoreLocalEntity = physicsBodiesMap.Lookup.TryGetValue(entity, out int physicsBody);
            ActorFilterInfo entityAInfo = Helpers.GetActorFilterInfo(entity, teams, firstInstigators, tileColliderTags);

            for (int i = 0; i < actionsOnOverlap.Length; i++)
            {
                var actionOnContact = actionsOnOverlap[i];

                PointDistanceInput pointDistance = new PointDistanceInput()
                {
                    Filter = CollisionFilter.ThatCollidesWith(actionOnContact.OverlapFilter),
                    MaxDistance = (float)actionOnContact.OverlapRadius,
                    Position = (float2)position.Value
                };

                if (ignoreLocalEntity)
                    pointDistance.Ignore = new IgnoreHit(physicsBody);

                if (physicsWorld.CalculateDistance(pointDistance, ref hits))
                {
                    foreach (var hit in hits)
                    {
                        if (mutedActions.IsMuted(entity, hit.Entity, actionOnContact.Data.Id))
                            continue;

                        ActorFilterInfo entityBInfo = Helpers.GetActorFilterInfo(hit.Entity, teams, firstInstigators, tileColliderTags);

                        if (Helpers.ActorFilterMatches(entityAInfo, entityBInfo, actionOnContact.Data.ActionFilter))
                        {
                            outActions.Add(new GameActionRequest()
                            {
                                Instigator = entity,
                                Target = hit.Entity,
                                ActionEntity = actionOnContact.Data.ActionEntity,
                            });

                            if (actionOnContact.Data.SameTargetCooldown > 0)
                            {
                                mutedActions.Add(new MutedContactActionElement()
                                {
                                    Instigator = entity,
                                    ContactActionBufferId = actionOnContact.Data.Id,
                                    ExpirationTime = actionOnContact.Data.SameTargetCooldown,
                                    Target = hit.Entity
                                });
                            }
                        }
                    }

                    hits.Clear();
                }
            }
        }).WithDisposeOnCompletion(hits)
        .Schedule();

        _gameActionSystem.HandlesToWaitFor.Add(Dependency);
    }
}

public class UpdateMutedContactActionSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        var time = Time.ElapsedTime;
        var mutedActions = GetSingletonBuffer<MutedContactActionElement>();
        for (int i = mutedActions.Length - 1; i >= 0; i--)
        {
            if (mutedActions[i].ExpirationTime >= time)
            {
                mutedActions.RemoveAt(i);
            }
        }
    }
}
