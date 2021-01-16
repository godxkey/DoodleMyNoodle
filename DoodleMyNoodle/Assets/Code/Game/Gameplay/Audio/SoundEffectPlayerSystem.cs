using System;
using UnityEngine;
using UnityEngineX;

public class SoundEffectPlayerSystem : GamePresentationSystem<SoundEffectPlayerSystem>
{
    [SerializeField] private AudioSource _audioSource;

    [SerializeField] private SoundEffectDescription _damageSound; // global for now but can be per character/ennemy

    public override void OnPostSimulationTick()
    {
        if (_audioSource == null)
            return;

        Cache.SimWorld.Entities.ForEach((ref GameActionEventData gameActionEvent) =>
        {
            SimWorld.TryGetComponentData(gameActionEvent.GameActionContext.Entity, out SimAssetId entitySimAssetID);

            GameObject entityPrefab = PresentationHelpers.FindSimAssetPrefab(entitySimAssetID);
            if (entityPrefab != null)
            {
                SoundEffectDescription soundEffect = entityPrefab.GetComponent<SoundEffectDescription>();
                if (soundEffect != null)
                {
                    soundEffect.SoundEffect.PlayOn(_audioSource);
                }
            }
        });

        Cache.SimWorld.Entities.ForEach((ref DamageEventData gameActionEvent) =>
        {
            if (_damageSound != null)
            {
                _damageSound.SoundEffect.PlayOn(_audioSource);
            }
        });
    }

    protected override void OnGamePresentationUpdate() { }
}