using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public abstract class GameEffectDisplayBase : GamePresentationBehaviour
{
    public Entity SimActorEntity;
    public Entity SimEffectEntity;
    [NonSerialized]
    public GameObject ViewRoot;
}

public class GameEffectDisplayManager : BindedPresentationEntityComponent
{
    [SerializeField] private Transform _container;

    private Dictionary<Entity, GameObject> _currentEffects = new Dictionary<Entity, GameObject>();

    public override void PresentationUpdate()
    {
        UpdateCurrentEffects();
    }

    void UpdateCurrentEffects()
    {
        using var _0 = HashSetPool<Entity>.Take(out var effectsToAdd);
        using var _1 = HashSetPool<Entity>.Take(out var effectsToRemove);
        using var _2 = ListPool<Entity>.Take(out var updatedEffects);

        // _________________________________________ Collect Updated Effects List _________________________________________ //
        if (SimWorld.TryGetBufferReadOnly(SimEntity, out DynamicBuffer<GameEffectBufferElement> simEffects))
        {
            foreach (var item in simEffects)
            {
                updatedEffects.Add(item.EffectEntity);
            }
        }

        // _________________________________________ Find Effects To Add / Remove _________________________________________ //
        foreach (var effectEntity in _currentEffects.Keys)
            effectsToRemove.Add(effectEntity);
        effectsToRemove.ExceptWith(updatedEffects);

        foreach (var effectEntity in updatedEffects)
            effectsToAdd.Add(effectEntity);
        effectsToAdd.ExceptWith(_currentEffects.Keys);

        // _________________________________________ Process Add _________________________________________ //
        foreach (var effectEntity in effectsToAdd)
        {
            GameObject effectView = null;

            if (SimWorld.TryGetComponent(effectEntity, out SimAssetId simAssetId))
            {
                var effectPrefab = PresentationHelpers.FindSimAssetPrefab(simAssetId);
                if (effectPrefab != null && effectPrefab.TryGetComponent(out GameEffectAuth gameEffectAuth) && gameEffectAuth.CharacterVFXPrefab != null)
                {
                    effectView = Instantiate(gameEffectAuth.CharacterVFXPrefab, _container);
                    var transform = effectView.GetComponent<Transform>();
                    transform.localPosition = Vector3.zero;
                    transform.localScale = Vector3.one;

                    if(effectView.TryGetComponent(out GameEffectDisplayBase effectDisplayComponent))
                    {
                        effectDisplayComponent.SimEffectEntity = effectEntity;
                        effectDisplayComponent.SimActorEntity = SimEntity;
                        effectDisplayComponent.ViewRoot = gameObject;
                    }
                }
            }
            _currentEffects.Add(effectEntity, effectView);
        }

        // _________________________________________ Process Remove _________________________________________ //
        foreach (var effectEntity in effectsToRemove)
        {
            GameObject effectView = _currentEffects[effectEntity];

            if (effectView != null)
            {
                Destroy(effectView);
            }

            _currentEffects.Remove(effectEntity);
        }
    }
}