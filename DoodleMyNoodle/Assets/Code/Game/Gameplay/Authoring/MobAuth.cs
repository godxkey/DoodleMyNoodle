using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class MobAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public float SpawnHeight = 0.5f; // used by the level generator

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<MobEnemyTag>(entity);
    }
}