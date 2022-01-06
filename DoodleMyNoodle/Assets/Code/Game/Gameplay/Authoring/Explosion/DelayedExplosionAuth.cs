using CCC.InspectorDisplay;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class DelayedExplosionAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public bool UseTime = true;
    [ShowIf("UseTime")]
    public int TimeDelay = 1;
    [HideIf("UseTime")]
    public int TurnDelay = 1;

    public fix Radius = 1;
    public int Damage = 1;
    public bool DestroyTiles = true;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new DelayedExplosion() 
        { 
            UseTime = UseTime,
            TimeDuration = TimeDelay,
            TurnDuration = TurnDelay,
            Radius = Radius,
            Damage = Damage,
            DestroyTiles = DestroyTiles
        });
    }
}