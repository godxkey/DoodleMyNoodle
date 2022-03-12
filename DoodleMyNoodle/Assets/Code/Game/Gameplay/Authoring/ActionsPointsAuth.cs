using CCC.InspectorDisplay;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class ActionsPointsAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public int MaxValue = 4;
    public bool StartGameAtMaxValue = true;
    public float RechargeRate = 1;
    public float RechargeDelay = 1;

    [HideIf(nameof(StartGameAtMaxValue))]
    public int StartValue = 4;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (StartGameAtMaxValue)
        {
            StartValue = MaxValue;
        }

        dstManager.AddComponentData(entity, new ActionPoints { Value = StartValue });
        dstManager.AddComponentData(entity, new MinimumFix<ActionPoints> { Value = 0 });
        dstManager.AddComponentData(entity, new MaximumFix<ActionPoints> { Value = MaxValue });
        dstManager.AddComponentData(entity, new ActionPointsRechargeRate { Value = (fix)RechargeRate });
        dstManager.AddComponentData(entity, new ActionPointsRechargeCooldown { Value = (fix)RechargeDelay, LastTime = fix.MinValue });
    }
}
