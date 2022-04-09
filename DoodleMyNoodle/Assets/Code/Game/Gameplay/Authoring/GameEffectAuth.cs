using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class GameEffectAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public float Duration;

    public GameObject GameActionOnBegin;
    public GameObject GameActionOnTick;
    public GameObject GameActionOnEnd;
    public GameObject GameActionOnInterrupt;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameEffect() { RemainingTime = (fix)Duration });

        if (GameActionOnBegin)
            dstManager.AddComponentData(entity, new GameEffectOnBeginGameAction() { Action = conversionSystem.GetPrimaryEntity(GameActionOnBegin.gameObject) });

        if (GameActionOnTick)
            dstManager.AddComponentData(entity, new GameEffectOnTickGameAction() { Action = conversionSystem.GetPrimaryEntity(GameActionOnTick.gameObject) });

        if (GameActionOnEnd)
            dstManager.AddComponentData(entity, new GameEffectOnEndGameAction() { Action = conversionSystem.GetPrimaryEntity(GameActionOnEnd.gameObject) });

        if (GameActionOnInterrupt)
            dstManager.AddComponentData(entity, new GameEffectOnInteruptGameAction() { Action = conversionSystem.GetPrimaryEntity(GameActionOnInterrupt.gameObject) });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        if (GameActionOnBegin)
            referencedPrefabs.Add(GameActionOnBegin);

        if (GameActionOnTick)
            referencedPrefabs.Add(GameActionOnTick);

        if (GameActionOnEnd)
            referencedPrefabs.Add(GameActionOnEnd);

        if (GameActionOnInterrupt)
            referencedPrefabs.Add(GameActionOnInterrupt);
    }
}