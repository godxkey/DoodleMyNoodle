using CCC.InspectorDisplay;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


[DisallowMultipleComponent]
[RequiresEntityConversion]
public class AIAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<AITag>(entity);
        dstManager.AddComponent<ControlledEntity>(entity);
        dstManager.AddComponentData(entity, new ReadyForNextTurn() { Value = false });
    }
}
