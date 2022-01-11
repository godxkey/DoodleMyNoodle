using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class TurnSystemSetting : MonoBehaviour, IConvertGameObjectToEntity
{
    public fix TurnDuration = 30;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new TurnSystemDataTimerSettings { TurnDuration = TurnDuration });
        dstManager.AddComponent<TurnSystemDataTag>(entity);
        dstManager.AddComponent<TurnSystemDataRemainingTurnTime>(entity);
        dstManager.AddComponent<TurnSystemDataCurrentTurnGroupIndex>(entity);
        dstManager.AddComponent<TurnSystemDataTurnTime>(entity);
        dstManager.AddComponent<TurnSystemDataRoundTime>(entity);
        dstManager.AddBuffer<TurnSystemDataRoundSequenceElement>(entity);
    }
}
