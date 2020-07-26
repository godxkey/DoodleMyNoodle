using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public class ItemContainerInteractableByClick : ObjectInteractableByClick
{
    protected override void InteractionTriggeredByInput()
    {
        InteractableInventoryDisplay.Instance.ShowAndSetupDisplay(SimEntity);
    }
}