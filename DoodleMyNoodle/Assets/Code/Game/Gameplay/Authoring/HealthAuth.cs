using CCC.InspectorDisplay;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class HealthAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public int MaxValue = 10;
    public bool StartAtMax = true;

    [HideIf(nameof(StartAtMax))]
    public int StartValue = 10;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Health { Value = StartAtMax ? MaxValue : StartValue });
        dstManager.AddComponentData(entity, new MinimumInt<Health> { Value = 0 });
        dstManager.AddComponentData(entity, new MaximumInt<Health> { Value = MaxValue });
    }
}
