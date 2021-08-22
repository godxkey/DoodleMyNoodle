using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class HookAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public fix TravelBackSpeed;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new HookData()
        {
            State = HookData.EState.Uninitialized,
            TravelBackSpeed = TravelBackSpeed,
        });
    }
}