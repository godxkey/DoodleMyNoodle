using Unity.Collections;
using Unity.Entities;

[UpdateInGroup(typeof(PreAISystemGroup))]
public class DestroyDanglingAIControllersSystem : SimSystemBase
{
    NativeList<Entity> _toDestroy;

    protected override void OnCreate()
    {
        base.OnCreate();
        _toDestroy = new NativeList<Entity>(Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _toDestroy.Dispose();
    }

    protected override void OnUpdate()
    {
        var pawns = GetComponentDataFromEntity<Controllable>(isReadOnly: true);

        // Destroy AIs that have no more pawns
        var toDestroy = _toDestroy;
        Entities
            .WithAll<AITag>()
            .ForEach((Entity controller, in ControlledEntity pawn) =>
            {
                if (!pawns.HasComponent(pawn))
                {
                    toDestroy.Add(controller);
                }
            }).Run();

        EntityManager.DestroyEntity(toDestroy);
        toDestroy.Clear();
    }
}

[UpdateInGroup(typeof(PreAISystemGroup))]
[UpdateBefore(typeof(DestroyDanglingAIControllersSystem))]
public class UnbindAIFromDeathPawnsSystem : SimSystemBase
{
    struct UnbindPair
    {
        public Entity Controller;
        public Entity Pawn;
    }

    NativeList<UnbindPair> _toUnbind;

    protected override void OnCreate()
    {
        base.OnCreate();
        _toUnbind = new NativeList<UnbindPair>(Allocator.Persistent);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        _toUnbind.Dispose();
    }

    protected override void OnUpdate()
    {
        var healths = GetComponentDataFromEntity<Health>(isReadOnly: true);
        var pawns = GetComponentDataFromEntity<Controllable>(isReadOnly: true);
        var aiTags = GetComponentDataFromEntity<AITag>(isReadOnly: true);
        var toUnbind = _toUnbind;

        Entities.ForEach((in DamageEventData damageEvent) =>
        {
            // entity dead ?
            if (healths.TryGetComponent(damageEvent.EntityDamaged, out Health health) && health <= 0)
            {
                // entity is controlled ?
                if (pawns.TryGetComponent(damageEvent.EntityDamaged, out Controllable controllable))
                {
                    // contorlled by AI ?
                    if (aiTags.HasComponent(controllable.CurrentController))
                    {
                        toUnbind.Add(new UnbindPair()
                        {
                            Controller = controllable.CurrentController,
                            Pawn = damageEvent.EntityDamaged
                        });
                    }
                }
            }
        }).Run();

        foreach (UnbindPair pair in toUnbind)
        {
            EntityManager.SetComponentData(pair.Controller, new ControlledEntity() { Value = Entity.Null });
            EntityManager.SetComponentData(pair.Pawn, new Controllable() { CurrentController = Entity.Null });
        }
        toUnbind.Clear();
    }
}
