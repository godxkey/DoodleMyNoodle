using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class AutoAttackAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public float AttackRate = 1;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData<AutoAttackRate>(entity, (fix)AttackRate);
        dstManager.AddComponentData<AutoAttackProgress>(entity, default);
        dstManager.AddComponentData<ShouldAutoAttack>(entity, default);
        dstManager.AddComponentData<AutoAttackAction>(entity, default);
    }
}
