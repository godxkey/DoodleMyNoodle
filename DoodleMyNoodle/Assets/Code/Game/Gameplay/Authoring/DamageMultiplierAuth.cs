using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class DamageMultiplierAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public fix DefaultDamageMultiplier = 1;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new BaseDamageMultiplier() { Value = DefaultDamageMultiplier });
        dstManager.AddComponentData(entity, new DamageMultiplier() { Value = DefaultDamageMultiplier });
    }
}