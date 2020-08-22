using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public class ItemContainerInteractableByClick : ObjectInteractableByClick
{
    protected override void OnInteractionTriggeredByInput()
    {
        InteractableInventoryDisplaySystem.Instance.SetupDisplayForInventory(SimEntity);
    }
}