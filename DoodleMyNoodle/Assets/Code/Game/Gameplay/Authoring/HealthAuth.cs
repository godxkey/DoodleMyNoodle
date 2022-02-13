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
    public HealthAuth Proxy;

    [HideIf(nameof(HasProxy))]
    public int MaxValue = 10;

    [HideIf(nameof(HasProxy))]
    public bool StartAtMax = true;

    [HideIf(nameof(HasProxy))]
    public bool DestroyOnDeath = false;

    [ShowIf(nameof(ShowStartValue))]
    public int StartValue = 10;

    [ShowIf(nameof(ShowTerrainOnlyColliderIfDead))]
    public bool TerrainOnlyColliderIfDead = true;

    private bool ShowStartValue => !StartAtMax && !HasProxy;
    private bool ShowTerrainOnlyColliderIfDead => GetComponent<PhysicsBodyAuth>() != null && !HasProxy;
    private bool HasProxy => Proxy != null;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (HasProxy)
        {
            dstManager.AddComponentData<HealthProxy>(entity, conversionSystem.GetPrimaryEntity(Proxy.gameObject));
        }
        else
        {
            dstManager.AddComponentData(entity, new Health { Value = StartAtMax ? MaxValue : StartValue });
            dstManager.AddComponentData(entity, new MinimumFix<Health> { Value = 0 });
            dstManager.AddComponentData(entity, new MaximumFix<Health> { Value = MaxValue });

            if (DestroyOnDeath)
                dstManager.AddComponent<DestroyOnDeath>(entity);

            if (ShowTerrainOnlyColliderIfDead && TerrainOnlyColliderIfDead)
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
}