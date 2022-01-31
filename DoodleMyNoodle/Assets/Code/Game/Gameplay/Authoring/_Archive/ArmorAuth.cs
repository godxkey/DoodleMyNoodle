using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class ArmorAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public int StartValue = 0;
    public int MinValue = 0;
    public int MaxValue = int.MaxValue;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Armor { Value = StartValue });
        dstManager.AddComponentData(entity, new MinimumInt<Armor> { Value = MinValue });
        dstManager.AddComponentData(entity, new MaximumInt<Armor> { Value = MaxValue });
    }
}
