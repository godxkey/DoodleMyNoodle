using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
public class GameEffectAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [Header("Simulation")]
    public float Duration;
    public GameObject GameActionOnBegin;
    public GameObject GameActionOnTick;
    public GameObject GameActionOnEnd;
    
    [Header("Presentation")]
    public GameObject CharacterVFXPrefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<GameEffectTag>(entity);
        dstManager.AddComponent<FirstInstigator>(entity);

        dstManager.AddComponentData(entity, new GameEffectRemainingDuration() { Value = (fix)Duration });

        if (GameActionOnBegin)
            dstManager.AddComponentData(entity, new GameEffectOnBeginGameAction() { Action = conversionSystem.GetPrimaryEntity(GameActionOnBegin.gameObject) });

        if (GameActionOnTick)
            dstManager.AddComponentData(entity, new GameEffectOnTickGameAction() { Action = conversionSystem.GetPrimaryEntity(GameActionOnTick.gameObject) });

        if (GameActionOnEnd)
            dstManager.AddComponentData(entity, new GameEffectOnEndGameAction() { Action = conversionSystem.GetPrimaryEntity(GameActionOnEnd.gameObject) });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        if (GameActionOnBegin)
            referencedPrefabs.Add(GameActionOnBegin);

        if (GameActionOnTick)
            referencedPrefabs.Add(GameActionOnTick);

        if (GameActionOnEnd)
            referencedPrefabs.Add(GameActionOnEnd);
    }
}