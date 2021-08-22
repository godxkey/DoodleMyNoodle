using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class LifetimeAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public fix Lifetime;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new RemainingLifetime()
        {
            Value = Lifetime
        });
    }
}