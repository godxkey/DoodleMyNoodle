using System;
using UnityEngine;
using UnityEngineX;
using DG.Tweening;
using Unity.Entities;
using Bolt.Utils;

public class DamageEventDisplaySystem : GamePresentationBehaviour
{
    public override void OnPostSimulationTick()
    {
        SimWorldCache.SimWorld.Entities.ForEach((ref DamageAppliedEventData damageApplyData) =>
        {
            GameObject presentationEntity = BindedSimEntityManaged.InstancesMap[damageApplyData.EntityDamaged];
            if (presentationEntity != null)
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

                entityRenderer.DOFade(0, 0.25f).SetLoops(6, LoopType.Yoyo).OnComplete(()=> { entityRenderer.SetAlpha(1); });
            }
        });
    }

    protected override void OnGamePresentationUpdate() { }
}