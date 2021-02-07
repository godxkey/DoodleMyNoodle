using CCC.Fix2D.Authoring;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public class ConvertToSimEntity : ConvertToEntityMultiWorld, IConvertGameObjectToEntity
{
    public override GameWorldType WorldToConvertTo => GameWorldType.Simulation;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (!gameObject.HasComponent<NoTransform>())
        {
            conversionSystem.World.GetExistingSystem<ConvertToFixTransformSystem>().ToConvert.Add(transform);
        }
    }
}

