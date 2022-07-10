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
    public float RechargeRate = 0;

    [HideIf(nameof(HasProxy))]
    public float RechargeCooldown = 4;

    [ShowIf(nameof(ShowStartValue), indent: false)]
    public int StartValue = 10;

    [ShowIf(nameof(ShowTerrainOnlyColliderIfDead), indent: false)]
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
            dstManager.AddComponentData<Health>(entity, (fix)(StartAtMax ? MaxValue : StartValue));
            dstManager.AddComponentData<BaseHealthMax>(entity, (fix)MaxValue);
            dstManager.AddComponentData<HealthMax>(entity, (fix)MaxValue);
            dstManager.AddComponentData<HealthRechargeRate>(entity, (fix)RechargeRate);
            dstManager.AddComponentData<HealthRechargeCooldown>(entity, (fix)RechargeCooldown);
            dstManager.AddComponentData<HealthLastHitTime>(entity, fix.MinValue);

            if (ShowTerrainOnlyColliderIfDead && TerrainOnlyColliderIfDead)
            {
                var normalCollider = dstManager.GetComponentData<PhysicsColliderBlob>(entity).Collider;

                if (!dstManager.TryGetComponent<ActorColliderRefs>(entity, out var actorColliderRefs))
                {
                    actorColliderRefs = new ActorColliderRefs(normalCollider);
                }

                // duplicate collider, but with alternate layer
                var deadCollider = BlobAssetReference<Collider>.Create(normalCollider.Value);
                deadCollider.Value.Filter = CollisionFilter.FromLayer(SimulationGameConstants.Physics.LAYER_CORPSES);

                actorColliderRefs.DeadCollider = deadCollider;

                dstManager.AddComponentData(entity, actorColliderRefs);
            }

        }
    }
}