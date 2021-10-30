using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class OnOverlapDamageOvertimeAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField] 
    private TimeValue Delay;

    [SerializeField] 
    private int Damage;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new OnOverlapDamageOvertimeSetting()
        {
            Delay = Delay,
            Damage = Damage
        });

        dstManager.AddComponentData(entity, new OnOverlapDamageOvertimeState() { TrackedTime = new TimeValue() { Type = Delay.Type, Value = Delay.Type == TimeValue.ValueType.Seconds ? (fix)dstManager.World.Time.ElapsedTime : 0 } });
        dstManager.AddBuffer<OnOverlapDamageOvertimeDamagedEntities>(entity);
    }
}