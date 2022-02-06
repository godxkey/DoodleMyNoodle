using System;
using UnityEngine;
using UnityEngineX;
using System.Collections.Generic;
using CCC.Fix2D;

public class GlobalVFXHandler : GamePresentationSystem<GlobalVFXHandler>
{
    public ItemUsedVFXDefinition ItemUsedVFX;

    protected override void OnGamePresentationUpdate()
    {
        foreach (var gameActionEvent in PresentationEvents.GameActionEvents.SinceLastPresUpdate)
        {
            // ITEM AUTH & ANIMATION TRIGGER
            SimWorld.TryGetComponent(gameActionEvent.Value.GameActionContext.Item, out SimAssetId instigatorAssetId);
            SimWorld.TryGetComponent(gameActionEvent.Value.GameActionContext.InstigatorPawn, out FixTranslation translation);
            GameObject instigatorPrefab = PresentationHelpers.FindSimAssetPrefab(instigatorAssetId);
            if (instigatorPrefab.TryGetComponent(out ItemAuth gameActionAuth))
            {
                if (gameActionAuth.Icon != null && ItemUsedVFX != null)
                {
                    ItemUsedVFX.TriggerVFX(new KeyValuePair<string, object>("Location", translation.Value.ToUnityVec())
                                         , new KeyValuePair<string, object>("Sprite", gameActionAuth.Icon));
                }
            }
        }
    }
}