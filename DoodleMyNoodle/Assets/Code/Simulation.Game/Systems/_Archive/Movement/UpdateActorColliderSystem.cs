using CCC.Fix2D;
using Unity.Entities;
using Unity.Jobs;

public struct ActorColliderRefs : IComponentData
{
    public BlobAssetReference<Collider> NormalCollider;
    public BlobAssetReference<Collider> AirControlCollider;
    public BlobAssetReference<Collider> DeadCollider;

    public ActorColliderRefs(BlobAssetReference<Collider> normalCollider)
    {
        NormalCollider = normalCollider;
        AirControlCollider = normalCollider;
        DeadCollider = normalCollider;
    }
}

[UpdateInGroup(typeof(MovementSystemGroup))]
[UpdateAfter(typeof(UpdateNavAgentFootingSystem))]
public class UpdateActorColliderSystem : SimGameSystemBase
{
    protected override void OnUpdate()
    {
        // depending on the actor state, update the collider

        // with health + navagent
        Entities
            .WithChangeFilter<NavAgentFootingState, Health>()
            .ForEach((ref PhysicsColliderBlob collider, in NavAgentFootingState footing, in ActorColliderRefs colliderRefs, in Health health) =>
            {
                UpdateActorCollider(ref collider, footing, colliderRefs, alive: health > 0);
            }).Run();

        // with health only
        Entities
            .WithNone<NavAgentFootingState>()
            .WithChangeFilter<Health>()
            .ForEach((ref PhysicsColliderBlob collider, in Health health, in ActorColliderRefs colliderRefs) =>
            {
                UpdateActorCollider(ref collider, footing: NavAgentFooting.Ground, colliderRefs, alive: health > 0);
            }).Run();

        // with navagent only
        Entities
            .WithNone<Health>()
            .WithChangeFilter<NavAgentFootingState>()
            .ForEach((ref PhysicsColliderBlob collider, in NavAgentFootingState footing, in ActorColliderRefs colliderRefs) =>
            {
                UpdateActorCollider(ref collider, footing, colliderRefs, alive: true);
            }).Run();
    }

    private static void UpdateActorCollider(ref PhysicsColliderBlob collider, in NavAgentFootingState footing, in ActorColliderRefs colliderRefs, bool alive)
    {
        // When pawn is in the air, reduce friction so we don't break on walls
        if (!alive)
        {
            collider.Collider = colliderRefs.DeadCollider;
        }
        else
        {
            collider.Collider = footing.Value == NavAgentFooting.AirControl
                ? colliderRefs.AirControlCollider
                : colliderRefs.NormalCollider;
        }
    }
}
