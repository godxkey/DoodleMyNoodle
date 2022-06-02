using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class VelocityChangeAfterLifetimeAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public float LifeTime = 1;
    public Vector2 VelocityMultipler;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new VelocityChangeAfterLifeTime()
        {
            LifeTimeToTrigger = (fix)LifeTime,
            VelocityMultiplier = VelocityMultipler.ToFixVec(),
            Applied = false
        });
        dstManager.AddComponentData(entity, new Lifetime());
    }
}