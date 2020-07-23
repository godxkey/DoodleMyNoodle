using System;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

public class ItemContainerInteractableByClick : ObjectInteractableByClick
{
    protected override void InteractionTriggeredByInput()
    {
        DynamicBuffer<InventoryItemPrefabReference> items = SimWorld.GetBufferReadOnly<InventoryItemPrefabReference>(SimEntity);

        string output = "Items in cache : ";
        foreach (var item in items)
        {
            output += item.ItemEntityPrefab.ToString() + " | ";
        }

        Debug.Log(output);
    }
}