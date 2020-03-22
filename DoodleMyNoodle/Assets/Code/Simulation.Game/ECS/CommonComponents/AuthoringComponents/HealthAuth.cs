using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class HealthAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public int StartValue = 10;
    public int MinValue = 0;
    public int MaxValue = 10;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Health { Value = StartValue });
        dstManager.AddComponentData(entity, new Minimum<Health> { Value = MinValue });
        dstManager.AddComponentData(entity, new Maximum<Health> { Value = MaxValue });
    }
}
