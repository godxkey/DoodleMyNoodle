using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;
using CCC.Fix2D;
using UnityEngineX;

[UpdateInGroup(typeof(PhysicsSystemGroup))]
[UpdateAfter(typeof(StepPhysicsWorldSystem)), UpdateBefore(typeof(EndFramePhysicsSystem))]
public class ExtractCollisionReactionsSystem : SimGameSystemBase
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
            World = _physicsWorldSystem.PhysicsWorld,
            OutActions = _reactSystem.ActionRequests,
            ActionOnContacts = GetBufferFromEntity<ActionOnContact>(isReadOnly: true),
            Teams = GetComponentDataFromEntity<Team>(isReadOnly: true),
            FirstInstigators = GetComponentDataFromEntity<FirstInstigator>(isReadOnly: true),
            TileColliderTags = GetComponentDataFromEntity<TileColliderTag>(isReadOnly: true),

        }.Schedule(_stepPhysicsWorldSystem.Simulation, ref _physicsWorldSystem.PhysicsWorld, Dependency);

        _endFramePhysicsSystem.HandlesToWaitFor.Add(Dependency);
    }

    struct ExtractFromEventsJob : ICollisionEventsJob
    {
        [ReadOnly] public BufferFromEntity<ActionOnContact> ActionOnContacts;
        [ReadOnly] public ComponentDataFromEntity<Team> Teams;
        [ReadOnly] public ComponentDataFromEntity<FirstInstigator> FirstInstigators;
        [ReadOnly] public ComponentDataFromEntity<TileColliderTag> TileColliderTags;
        [ReadOnly] public PhysicsWorld World;

        public NativeList<(Entity instigator, Entity target, ActionOnContact action)> OutActions;


        private struct EntityInfo
        {
            public Team Team;
            public bool IsTerrain;
        }

        public void Execute(CollisionEvent collisionEvent)
        {
            ProcessEntityPair(collisionEvent.EntityA, collisionEvent.EntityB, ref collisionEvent);
            ProcessEntityPair(collisionEvent.EntityB, collisionEvent.EntityA, ref collisionEvent);
        }

        private void ProcessEntityPair(Entity entityA, Entity entityB, ref CollisionEvent collisionEvent)
        {
            if (ActionOnContacts.HasComponent(entityA))
            {
                DynamicBuffer<ActionOnContact> actions = ActionOnContacts[entityA];
                EntityInfo entityAInfo = GetEntityInfo(entityA);
                EntityInfo entityBInfo = GetEntityInfo(entityB);

                for (int i = 0; i < actions.Length; i++)
                {
                    ActionOnContact actionOnContact = actions[i];
                    if (CanTrigger(ref entityAInfo, ref entityBInfo, ref actionOnContact))
                    {
                        OutActions.Add((entityA, entityB, actionOnContact));
                    }
                }
            }
        }

        private bool CanTrigger(ref EntityInfo entityAInfo, ref EntityInfo entityBInfo, ref ActionOnContact actionOnContact)
        {
            if ((actionOnContact.ActionFilter & ActionOnContact.Filter.Allies) != 0 && entityAInfo.Team == entityBInfo.Team)
                return true;
            if ((actionOnContact.ActionFilter & ActionOnContact.Filter.Enemies) != 0 && entityAInfo.Team != entityBInfo.Team)
                return true;
            if ((actionOnContact.ActionFilter & ActionOnContact.Filter.Terrain) != 0 && entityBInfo.IsTerrain)
                return true;
            return false;
        }

        EntityInfo GetEntityInfo(Entity entity)
        {
            EntityInfo result = new EntityInfo()
            {
                Team = Team.None,
                IsTerrain = false,
            };

            // try to get the team
            if (Teams.TryGetComponent(entity, out Team team))
            {
                result.Team = team;
            }
            else if (FirstInstigators.TryGetComponent(entity, out FirstInstigator firstInstigator))
            {
                if (Teams.TryGetComponent(firstInstigator, out Team firstInstigatorTeam))
                {
                    result.Team = firstInstigatorTeam;
                }
            }

            result.IsTerrain = TileColliderTags.HasComponent(entity);

            return result;
        }
    }
}

[UpdateInGroup(typeof(PostPhysicsSystemGroup))]
public class ReactOnCollisionSystem : SimGameSystemBase
{
    public NativeList<(Entity instigator, Entity target, ActionOnContact action)> ActionRequests;

    protected override void OnCreate()
    {
        base.OnCreate();
        ActionRequests = new NativeList<(Entity, Entity, ActionOnContact)>(Allocator.Persistent);

        RequireSingletonForUpdate<GridInfo>();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        ActionRequests.Dispose();
    }

    protected override void OnUpdate()
    {
        if (ActionRequests.Length > 0)
        {
            foreach (var request in ActionRequests)
            {
                CommonWrites.ExecuteGameAction(Accessor, request.instigator, request.action.ActionEntity, request.target);
            }
            ActionRequests.Clear();
        }
    }
}
