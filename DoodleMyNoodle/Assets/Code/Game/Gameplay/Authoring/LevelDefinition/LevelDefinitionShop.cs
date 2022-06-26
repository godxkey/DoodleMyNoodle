using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent, RequireComponent(typeof(LevelDefinition))]
public class LevelDefinitionShop : MonoBehaviour, IConvertGameObjectToEntity
{
    public float Duration = 30;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<LevelDefinitionPitStopTag>(entity);
        dstManager.AddComponentData<LevelDefinitionDuration>(entity, (fix)Duration);
    }
}