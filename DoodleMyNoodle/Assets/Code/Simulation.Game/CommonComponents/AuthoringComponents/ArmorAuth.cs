using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ArmorAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public int StartValue = 0;
    public int MinValue = 0;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Armor { Value = StartValue });
        dstManager.AddComponentData(entity, new MinimumInt<Armor> { Value = MinValue });
    }
}
