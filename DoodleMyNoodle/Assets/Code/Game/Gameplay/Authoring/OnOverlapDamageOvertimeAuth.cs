using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Serialization;

[DisallowMultipleComponent]
public class OnOverlapDamageOvertimeAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField, FormerlySerializedAs("Delay")]
    private TimeValue _delay;

    [SerializeField, FormerlySerializedAs("Damage")]
    private int _damage;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new OnOverlapDamageOvertimeSetting()
        {
            Delay = _delay,
            Damage = _damage
        });

        dstManager.AddComponentData(entity, new OnOverlapDamageOvertimeState()
        {
            TrackedTime = new TimeValue() { Type = _delay.Type, Value = 0 }
        });
        dstManager.AddBuffer<OnOverlapDamageOvertimeDamagedEntities>(entity);
    }
}