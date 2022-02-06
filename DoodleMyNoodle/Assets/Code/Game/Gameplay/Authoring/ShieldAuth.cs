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

    public int RechargeRate = 1;
    public float RechargeCooldown = 1;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity,
        new Shield 
        { 
            Value = StartAtMax ? MaxValue : StartValue, 
            RechargeRate = RechargeRate, 
            RechargeCooldown = (fix)RechargeCooldown 
        });

        dstManager.AddComponentData(entity, new MinimumInt<Shield> { Value = 0 });
        dstManager.AddComponentData(entity, new MaximumInt<Shield> { Value = MaxValue });
    }
}

