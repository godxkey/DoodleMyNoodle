using CCC.InspectorDisplay;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class ShieldAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public int MaxValue = 10;
    public bool StartAtMax = true;

    [HideIf(nameof(StartAtMax))]
    public int StartValue = 10;

    public float RechargeRate = 1;
    public float RechargeCooldown = 1;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData<Shield>(entity, (fix)(StartAtMax ? MaxValue : StartValue));
        dstManager.AddComponentData<ShieldMax>(entity, (fix)MaxValue);
        dstManager.AddComponentData<ShieldRechargeRate>(entity, (fix)RechargeRate);
        dstManager.AddComponentData<ShieldRechargeCooldown>(entity, (fix)RechargeCooldown);
        dstManager.AddComponentData<ShieldLastHitTime>(entity, fix.MinValue);
    }
}

