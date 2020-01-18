using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    public GameObject InventoryTileContainer;
    public GameObject InventoryTilePrefab;

    private bool _hasBeenSetup = false;

    public void ToggleInventory()
    {
        if (!_hasBeenSetup)
        {
            _hasBeenSetup = true;

            SimPawnComponent playerPawn = SimPawnHelpers.GetPawnFromController(PlayerIdHelpers.GetLocalSimPlayerComponent());

            SimInventoryComponent inventory = playerPawn.GetComponent<SimInventoryComponent>();

            for (int i = 0; i < inventory.InventorySize; i++)
            {
                SimItem item = inventory.GetItem(i);
                UIInventorySlot newSlot = Instantiate(InventoryTilePrefab, InventoryTileContainer.transform).GetComponent<UIInventorySlot>();
                newSlot.Init(item);
            }
        }
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
