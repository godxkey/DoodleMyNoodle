using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class StatModifiersAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public List<StatModifierType> StartingStatModifiers = new List<StatModifierType>();

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddBuffer<StatModifier>(entity);

        var entities = dstManager.AddBuffer<StartingStatModifier>(entity);

        foreach (var statusEffect in StartingStatModifiers)
        {
            entities.Add(new StartingStatModifier() { Type = statusEffect });
        }
    }
}