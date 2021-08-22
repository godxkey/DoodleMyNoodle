using UnityEngine;
using UnityEngineX;
using DG.Tweening;
using System.Collections.Generic;

public class DamageEventDisplaySystem : GamePresentationSystem<DamageEventDisplaySystem>
{
    public VFXDefinition DamageImpactVFX;

    public override void OnPostSimulationTick()
    {
        Cache.SimWorld.Entities.ForEach((ref DamageEventData damageApplyData) =>
        {
            DamageImpactVFX?.TriggerVFX(new KeyValuePair<string, object>("Location", damageApplyData.Position.ToUnityVec()));

            if (BindedSimEntityManaged.InstancesMap.TryGetValue(damageApplyData.EntityDamaged, out GameObject presentationEntity) && presentationEntity)
            {
                DoodleDisplay playerCharacterDoodleDisplay = presentationEntity.GetComponent<DoodleDisplay>();
                SpriteRenderer entityRenderer = null;
                if (playerCharacterDoodleDisplay != null)
                {
                    entityRenderer = playerCharacterDoodleDisplay.SpriteRenderer;
                }
                else
                {
                    entityRenderer = presentationEntity.GetComponentInChildren<SpriteRenderer>();
                }

                if (entityRenderer == null)
                {
                    Log.Error("No Sprite Rendered Found on Entity VE");
                    return;
                }

                Sequence sq = DOTween.Sequence();
                sq.AppendCallback(() => { if (entityRenderer) entityRenderer.SetAlpha(0); });
                sq.AppendInterval(0.1f);
                sq.AppendCallback(() => { if (entityRenderer) entityRenderer.SetAlpha(1); });
                sq.AppendInterval(0.1f);
                sq.SetLoops(6);
            }
        });
    }

    protected override void OnGamePresentationUpdate() { }
}