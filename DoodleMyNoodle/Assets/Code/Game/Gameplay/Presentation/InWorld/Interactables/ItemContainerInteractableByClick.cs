using System;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;
using CCC.Fix2D;

public class ItemContainerInteractableByClick : ObjectInteractableByClick
{
    private bool _inventoryDisplayed = false;

    protected override void OnInteractionTriggered()
    {
        if (Cache.LocalPawn == Entity.Null || SimEntity == Entity.Null)
            return;

        if (SimWorld.TryGetComponentData(SimEntity, out Interacted interacted))
        {
            if (Cache.LocalPawn == interacted.Instigator)
            {
                _inventoryDisplayed = true;
                InteractableInventoryDisplaySystem.Instance.SetupDisplayForInventory(SimEntity);
            }
        }
    }

    protected override void OnGamePresentationUpdate() 
    {
        if (Cache.LocalPawn == Entity.Null || SimEntity == Entity.Null || !InteractableInventoryDisplaySystem.Instance.IsOpen())
        {
            _inventoryDisplayed = false;
            return;
        }

        if (SimWorld.TryGetComponentData(SimEntity, out Interacted interacted))
        {
            if (_inventoryDisplayed && Cache.LocalPawn == interacted.Instigator)
            {
                fix2 itemContainerPosition = SimWorld.GetComponentData<FixTranslation>(SimEntity);
                fix2 localPawnPosition = SimWorld.GetComponentData<FixTranslation>(Cache.LocalPawn);

                fix interactionTileRange = SimWorld.GetComponentData<Interactable>(SimEntity).Range;

                int tilesBetween = fix.RoundToInt(fix.Abs((itemContainerPosition.x - localPawnPosition.x) + (itemContainerPosition.y - localPawnPosition.y)));

                if ((tilesBetween > interactionTileRange) && InteractableInventoryDisplaySystem.Instance.IsOpen())
                {
                    _inventoryDisplayed = false;
                    InteractableInventoryDisplaySystem.Instance.CloseDisplay();
                }
            }
        }
    }
}