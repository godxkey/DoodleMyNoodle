using System;
using Unity.Entities;
using UnityEngine;

[Serializable]
public struct Velocity : IComponentData
{
    public fix3 Value;
}

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class VelocityAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new Velocity { Value = new fix3(0,0,0) });
        dstManager.AddComponentData(entity, new PotentialNewTranslation { Value = new fix3(0, 0, 0) });
    }
}
