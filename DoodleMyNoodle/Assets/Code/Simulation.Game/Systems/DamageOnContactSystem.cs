using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using static fixMath;
using static Unity.Mathematics.math;

public class DamageOnContactSystem : SimComponentSystem
{
    private NativeList<Entity> _toDestroy;

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
        Entities
            .ForEach((ref TileActorOverlapBeginEventData triggerEvent) =>
            {
                ProcessEntityPair(triggerEvent.TileActorA, triggerEvent.TileActorB, _toDestroy);
                ProcessEntityPair(triggerEvent.TileActorB, triggerEvent.TileActorA, _toDestroy);
            });

        EntityManager.DestroyEntity(_toDestroy);
        _toDestroy.Clear();
    }

    private void ProcessEntityPair(Entity instigatorPawn, Entity targetPawn, NativeList<Entity> toDestroy)
    {
        if (EntityManager.TryGetComponentData(instigatorPawn, out DamageOnContact damageOnContact))
        {
            if (EntityManager.HasComponent<Health>(targetPawn))
            {
                CommonWrites.RequestDamageOnTarget(Accessor, instigatorPawn, targetPawn, damageOnContact.Value);
                
                if (damageOnContact.DestroySelf)
                {
                    toDestroy.AddUnique(instigatorPawn);
                }
            }

        }
    }
}
