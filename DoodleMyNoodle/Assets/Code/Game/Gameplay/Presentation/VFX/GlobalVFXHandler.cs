using System;
using UnityEngine;
using UnityEngineX;
using System.Collections.Generic;
using CCC.Fix2D;
using Unity.Entities;

public class GlobalVFXHandler : GamePresentationSystem<GlobalVFXHandler>
{
    public ItemUsedVFXDefinition ItemUsedVFX;

    public override void PresentationUpdate()
    {
        foreach (var gameActionEvent in PresentationEvents.GameActionEvents.SinceLastPresUpdate)
        {
            SimWorld.TryGetComponent(gameActionEvent.GameActionContext.LastPhysicalInstigator, out FixTranslation lastPhysicalInstigatorTranslation);

            // ITEM USED VFX
            // only do the vfx if not an auto attack
            if (!SimWorld.HasComponent<PeriodicAction>(gameActionEvent.GameActionContext.ActionInstigatorActor) ||
                (SimWorld.TryGetComponent(gameActionEvent.GameActionContext.ActionInstigatorActor, out PeriodicAction autoAttack) && autoAttack.Value != gameActionEvent.GameActionContext.Action))
            {
                SimWorld.TryGetComponent(gameActionEvent.GameActionContext.ActionInstigatorActor, out SimAssetId instigatorAssetId);
                GameObject itemPrefab = PresentationHelpers.FindSimAssetPrefab(instigatorAssetId);
                if (itemPrefab != null &&
                    itemPrefab.TryGetComponent(out ItemAuth itemAuth))
                {
                    if (itemAuth.Icon != null && ItemUsedVFX != null)
                    {
                        ItemUsedVFX.TriggerVFX(new KeyValuePair<string, object>("Location", lastPhysicalInstigatorTranslation.Value.ToUnityVec())
                                             , new KeyValuePair<string, object>("Sprite", itemAuth.Icon));
                    }
                }
            }

            // GAME ACTION USED VFX
            SimWorld.TryGetComponent(gameActionEvent.GameActionContext.Action, out SimAssetId gameActionAssetId);
            GameObject gameActionPrefab = PresentationHelpers.FindSimAssetPrefab(gameActionAssetId);
            if (gameActionPrefab != null && gameActionPrefab.TryGetComponent(out GameActionAuth gameActionAuth))
            {
                VFXDefinition instigatorVFX = gameActionAuth.InstigatorVFX;

                if (instigatorVFX != null)
                {
                    instigatorVFX.TriggerVFX(new KeyValuePair<string, object>("Location", lastPhysicalInstigatorTranslation.Value.ToUnityVec()));
                }

                VFXDefinition targetsVFX = gameActionAuth.TargetsVFX;
                if (targetsVFX != null && gameActionEvent.GameActionContext.Targets.IsCreated)
                {
                    for (int i = 0; i < gameActionEvent.GameActionContext.Targets.Length; i++)
                    {
                        Entity target = gameActionEvent.GameActionContext.Targets[i];
                        if (SimWorld.TryGetComponent(target, out FixTranslation targetLocation))
                        {
                            targetsVFX.TriggerVFX(new KeyValuePair<string, object>("Location", targetLocation.Value.ToUnityVec()));
                        }
                    }
                }
            }
        }
    }
}