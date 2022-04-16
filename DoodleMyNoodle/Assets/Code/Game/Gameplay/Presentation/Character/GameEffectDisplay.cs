using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public class GameEffectDisplay : BindedPresentationEntityComponent
{
    [SerializeField] private Transform _container;

    private Dictionary<SimAssetId, GameObject> _currentEffects = new Dictionary<SimAssetId, GameObject>();

    public override void PresentationUpdate()
    {
        UpdateCurrentEffects();
    }

    void UpdateCurrentEffects()
    {
        using var _0 = HashSetPool<SimAssetId>.Take(out var effectsToAdd);
        using var _1 = HashSetPool<SimAssetId>.Take(out var effectsToRemove);
        using var _2 = ListPool<SimAssetId>.Take(out var updatedEffects);

        // _________________________________________ Collect Updated Effects List _________________________________________ //
        if (SimWorld.TryGetBufferReadOnly(SimEntity, out DynamicBuffer<GameEffectBufferElement> simEffects))
        {
            foreach (var item in simEffects)
            {
                if (SimWorld.TryGetComponent<SimAssetId>(item.EffectEntity, out var effectSimAssetId))
                {
                    updatedEffects.Add(effectSimAssetId);
                }
            }
        }

        // _________________________________________ Find Effects To Add / Remove _________________________________________ //
        foreach (var item in _currentEffects.Keys)
            effectsToRemove.Add(item);
        effectsToRemove.ExceptWith(updatedEffects);

        foreach (var item in updatedEffects)
            effectsToAdd.Add(item);
        effectsToAdd.ExceptWith(_currentEffects.Keys);

        // _________________________________________ Process Add _________________________________________ //
        foreach (var simAsset in effectsToAdd)
        {
            GameObject effectView = null;

            var effectPrefab = PresentationHelpers.FindSimAssetPrefab(simAsset);
            if (effectPrefab != null && effectPrefab.TryGetComponent(out GameEffectAuth gameEffectAuth) && gameEffectAuth.CharacterVFXPrefab != null)
            {
                effectView = Instantiate(gameEffectAuth.CharacterVFXPrefab, _container);
                var transform = effectView.GetComponent<Transform>();
                transform.localPosition = Vector3.zero;
                transform.localScale = Vector3.one;
            }
            _currentEffects.Add(simAsset, effectView);
        }

        // _________________________________________ Process Remove _________________________________________ //
        foreach (var simAsset in effectsToRemove)
        {
            GameObject effectView = _currentEffects[simAsset];

            if (effectView != null)
            {
                Destroy(effectView);
            }

            _currentEffects.Remove(simAsset);
        }
    }
}