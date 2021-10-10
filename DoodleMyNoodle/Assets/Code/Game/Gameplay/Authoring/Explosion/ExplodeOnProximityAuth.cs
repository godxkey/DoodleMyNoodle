using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class ExplodeOnProximityAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField]
    private float Distance;

    [SerializeField]
    private float Radius;

    [SerializeField]
    private int Damage;

    [SerializeField]
    private bool DestroyTiles;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ExplodeOnProximity()
        {
            Distance = (fix)Distance,
            Radius = (fix)Radius,
            Damage = Damage,
            DestroyTiles = DestroyTiles,
            Activated = false
        });
    }
}