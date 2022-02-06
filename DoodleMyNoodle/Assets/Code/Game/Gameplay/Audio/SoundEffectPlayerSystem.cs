using System;
using UnityEngine;
using UnityEngineX;

public class SoundEffectPlayerSystem : GamePresentationSystem<SoundEffectPlayerSystem>
{
    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private AudioPlayable _damageSound; // global for now but can be per character/ennemy

    protected override void OnGamePresentationUpdate()
    {
        if (_audioSource == null)
            return;

        // Item sounds
        foreach (var gameActionEvent in PresentationEvents.GameActionEvents.SinceLastPresUpdate)
        {
            SimWorld.TryGetComponent(gameActionEvent.GameActionContext.Item, out SimAssetId entitySimAssetID);

            GameObject entityPrefab = PresentationHelpers.FindSimAssetPrefab(entitySimAssetID);
            if (entityPrefab != null)
            {
                var sfx = entityPrefab.GetComponent<ItemAuth>()?.SfxOnUse;
                if (sfx != null)
                {
                    sfx.PlayOn(_audioSource);
                }
            }
        }

        // Damage sounds
        Cache.SimWorld.Entities.ForEach((ref DamageEventData gameActionEvent) =>
        {
            if (_damageSound != null)
            {
                _damageSound.PlayOn(_audioSource);
            }
        });
    }
}