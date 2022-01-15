using CCC.Fix2D;
using CCC.Fix2D.Authoring;
using CCC.InspectorDisplay;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using Collider = CCC.Fix2D.Collider;

[DisallowMultipleComponent]
public class HealthAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public int MaxValue = 10;
    public bool StartAtMax = true;
    public bool DestroyOnDeath = false;

    [HideIf(nameof(StartAtMax))]
    public int StartValue = 10;

    [ShowIf(nameof(HasPhysicsBody))]
    public bool TerrainOnlyColliderIfDead = true;

    private bool HasPhysicsBody => GetComponent<PhysicsBodyAuth>() != null;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Health { Value = StartAtMax ? MaxValue : StartValue });
        dstManager.AddComponentData(entity, new MinimumInt<Health> { Value = 0 });
        dstManager.AddComponentData(entity, new MaximumInt<Health> { Value = MaxValue });

        if (DestroyOnDeath)
            dstManager.AddComponent<DestroyOnDeath>(entity);

        if (HasPhysicsBody && TerrainOnlyColliderIfDead)
        {
            var normalCollider = dstManager.GetComponentData<PhysicsColliderBlob>(entity).Collider;

            if (!dstManager.TryGetComponentData<ActorColliderRefs>(entity, out var actorColliderRefs))
            {
                actorColliderRefs = new ActorColliderRefs(normalCollider);
            }

            // duplicate collider, but with alternate layer
            var deadCollider = BlobAssetReference<Collider>.Create(normalCollider.Value);
            deadCollider.Value.Filter = CollisionFilter.FromLayer(SimulationGameConstants.Physics.LAYER_CONTACT_WITH_TERRAIN_ONLY);

            actorColliderRefs.DeadCollider = deadCollider;

            dstManager.SetOrAddComponentData(entity, actorColliderRefs);
        }
    }
}
