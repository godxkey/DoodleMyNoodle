using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ActionRefillSetting : MonoBehaviour, IConvertGameObjectToEntity
{
    public int AmountToAdd = 0;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new ActionRefillAmount { Value = AmountToAdd });
    }
}
