using UnityEngine;
using UnityEngineX;
using DG.Tweening;
using System.Collections.Generic;

public class DamageEventDisplaySystem : GamePresentationSystem<DamageEventDisplaySystem>
{
    protected override void OnGamePresentationUpdate()
    {
        foreach (var hpDeltaEvent in PresentationEvents.HealthDeltaEvents.SinceLastPresUpdate)
        {
            if (hpDeltaEvent.IsDamage
                && BindedSimEntityManaged.InstancesMap.TryGetValue(hpDeltaEvent.AffectedEntity, out GameObject presentationEntity)
                && presentationEntity)
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
        }
    }
}