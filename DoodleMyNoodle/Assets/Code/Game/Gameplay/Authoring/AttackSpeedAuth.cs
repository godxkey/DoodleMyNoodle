using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class AttackSpeedAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public fix DefaultAttackSpeed = 1;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new BaseAttackSpeed() { Value = DefaultAttackSpeed });
        dstManager.AddComponentData(entity, new AttackSpeed() { Value = DefaultAttackSpeed });
    }
}