using System;
using UnityEngine;
using UnityEngineX;

public class SoundEffectPlayerSystem : GamePresentationSystem<SoundEffectPlayerSystem>
{
    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private AudioPlayable _damageSound; // global for now but can be per character/ennemy

    public override void OnPostSimulationTick()
    {
        if (_audioSource == null)
            return;

        // Item sounds
        Cache.SimWorld.Entities.ForEach((ref GameActionEventData gameActionEvent) =>
        {
            SimWorld.TryGetComponentData(gameActionEvent.GameActionContext.Entity, out SimAssetId entitySimAssetID);

            GameObject entityPrefab = PresentationHelpers.FindSimAssetPrefab(entitySimAssetID);
            if (entityPrefab != null)
            {
                var sfx = entityPrefab.GetComponent<ItemAuth>()?.SfxOnUse;
                if (sfx != null)
                {
                    sfx.PlayOn(_audioSource);
                }
            }
        });

        // Damage sounds
        Cache.SimWorld.Entities.ForEach((ref DamageEventData gameActionEvent) =>
        {
            if (_damageSound != null)
            {
                _damageSound.PlayOn(_audioSource);
            }
        });
    }

    protected override void OnGamePresentationUpdate() { }
}