using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class DamageMultiplierAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public float BaseDamageMultiplier = 1;
    public float BaseDamageReceivedMultiplier = 1;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new BaseDamageMultiplier() { Value = (fix)BaseDamageMultiplier });
        dstManager.AddComponentData(entity, new DamageMultiplier() { Value = (fix)BaseDamageMultiplier });
        dstManager.AddComponentData(entity, new BaseDamageReceivedMultiplier() { Value = (fix)BaseDamageReceivedMultiplier });
        dstManager.AddComponentData(entity, new DamageReceivedMultiplier() { Value = (fix)BaseDamageReceivedMultiplier });
    }
}