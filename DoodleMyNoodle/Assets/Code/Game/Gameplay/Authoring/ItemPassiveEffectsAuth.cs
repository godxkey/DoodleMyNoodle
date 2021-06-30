using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class ItemPassiveEffectsAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public List<string> Values;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        DynamicBuffer<ItemPassiveEffectId> itemPassiveEffects = dstManager.AddBuffer<ItemPassiveEffectId>(entity);
        foreach (string Value in Values)
        {
            itemPassiveEffects.Add(ItemPassiveEffectBank.GetItemPassiveEffectId(Value));
        }
    }
}