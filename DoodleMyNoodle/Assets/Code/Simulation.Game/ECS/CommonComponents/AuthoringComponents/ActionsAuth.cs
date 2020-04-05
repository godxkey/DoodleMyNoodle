using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ActionsAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public int StartValue = 4;
    public int MaxValue = 4;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ActionPoints { Value = StartValue });
        dstManager.AddComponentData(entity, new MinimumInt<ActionPoints> { Value = 0 });
        dstManager.AddComponentData(entity, new MaximumInt<ActionPoints> { Value = MaxValue });
    }
}
