using CCC.Fix2D;
using Unity.Entities;
using Unity.Jobs;

public struct ActorColliderRefs : IComponentData
{
    public BlobAssetReference<Collider> GroundedCollider;
    public BlobAssetReference<Collider> UngroundedCollider;
    public BlobAssetReference<Collider> DeadCollider;

    public ActorColliderRefs(BlobAssetReference<Collider> normalCollider)
    {
        GroundedCollider = normalCollider;
        UngroundedCollider = normalCollider;
        DeadCollider = normalCollider;
    }
}

[UpdateInGroup(typeof(MovementSystemGroup))]
public partial class UpdateActorColliderSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        // depending on the actor state, update the collider

        // with health + grounded
        Entities
            .WithChangeFilter<Health, Grounded>()
            .ForEach((ref PhysicsColliderBlob collider, in Health health, in Grounded grounded, in ActorColliderRefs colliderRefs) =>
            {
                UpdateActorCollider(ref collider, grounded, colliderRefs, alive: health > fix.Zero);
            }).Run();

        // with grounded only
        Entities
            .WithNone<Health>()
            .WithChangeFilter<Grounded>()
            .ForEach((ref PhysicsColliderBlob collider, in Grounded grounded, in ActorColliderRefs colliderRefs) =>
            {
                UpdateActorCollider(ref collider, grounded, colliderRefs, alive: true);
            }).Run();
    }

    private static void UpdateActorCollider(ref PhysicsColliderBlob collider, bool grounded, in ActorColliderRefs colliderRefs, bool alive)
    {
        // When pawn is in the air, reduce friction so we don't break on walls
        if (!alive)
        {
            collider.Collider = colliderRefs.DeadCollider;
        }
        else
        {
            collider.Collider = grounded
                ? colliderRefs.GroundedCollider
                : colliderRefs.UngroundedCollider;
        }
    }
}
