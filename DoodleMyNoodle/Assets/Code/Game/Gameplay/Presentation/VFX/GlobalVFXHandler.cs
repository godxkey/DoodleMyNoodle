using System;
using UnityEngine;
using UnityEngineX;
using System.Collections.Generic;
using CCC.Fix2D;
using Unity.Entities;

public class GlobalVFXHandler : GamePresentationSystem<GlobalVFXHandler>
{
    public ItemUsedVFXDefinition ItemUsedVFX;

    protected override void OnGamePresentationUpdate()
    {
        foreach (var gameActionEvent in PresentationEvents.GameActionEvents.SinceLastPresUpdate)
        {
            SimWorld.TryGetComponent(gameActionEvent.GameActionContext.LastPhysicalInstigator, out FixTranslation lastPhysicalInstigatorTranslation);

            // ITEM USED VFX
            // only do the vfx if not an auto attack
            if (!SimWorld.HasComponent<AutoAttackAction>(gameActionEvent.GameActionContext.ActionInstigatorActor) || 
                (SimWorld.TryGetComponent(gameActionEvent.GameActionContext.ActionInstigatorActor, out AutoAttackAction autoAttack) && autoAttack.Value != gameActionEvent.GameActionContext.Action))
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
                VFXDefinition InstigatorVFX = gameActionAuth.InstigatorVFX;
                if (InstigatorVFX != null)
                {
                    InstigatorVFX.TriggerVFX(new KeyValuePair<string, object>("Location", lastPhysicalInstigatorTranslation.Value.ToUnityVec()));
                }

                VFXDefinition TargetsVFX = gameActionAuth.TargetsVFX;
                if (TargetsVFX != null)
                {
                    foreach (Entity target in gameActionEvent.GameActionContext.Targets)
                    {
                        if (SimWorld.TryGetComponent(target, out FixTranslation targetLocation))
                        {
                            TargetsVFX.TriggerVFX(new KeyValuePair<string, object>("Location", targetLocation.Value.ToUnityVec()));
                        }
                        
                    }
                }
            }
        }
    }
}