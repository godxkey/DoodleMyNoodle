using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class DestroyAfterDelayAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField]
    private TimeValue Delay;

    public bool InitOnStart = false;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        fix StartTime = 0;

        if (Delay.Type == TimeValue.ValueType.Seconds)
        {
            StartTime = (fix)dstManager.World.Time.ElapsedTime;
        }

        dstManager.AddComponentData(entity, new DestroyAfterDelay()
        {
            Delay = Delay,
            TrackedTime = InitOnStart ? new TimeValue() { Type = Delay.Type, Value = StartTime } : new TimeValue() { Type = Delay.Type, Value = -1 }
        }); ;
    }
}