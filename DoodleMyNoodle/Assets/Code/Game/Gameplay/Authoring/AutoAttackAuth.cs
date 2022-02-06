using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX.InspectorDisplay;

[DisallowMultipleComponent]
public class AutoAttackAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    [Suffix("/s")]
    public float AttackRate = 1;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData<AutoAttackRate>(entity, (fix)AttackRate);
        dstManager.AddComponentData<AutoAttackProgress>(entity, default);
        dstManager.AddComponentData<ShouldAutoAttack>(entity, default);
        dstManager.AddComponentData<AutoAttackAction>(entity, default);
    }
}
