using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class StatusEffectsAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public List<StatusEffectType> StartingStatusEffects = new List<StatusEffectType>();

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddBuffer<StatusEffect>(entity);

        var entities = dstManager.AddBuffer<StartingStatusEffect>(entity);

        foreach (var statusEffect in StartingStatusEffects)
        {
            entities.Add(new StartingStatusEffect() { Type = statusEffect });
        }
    }
}