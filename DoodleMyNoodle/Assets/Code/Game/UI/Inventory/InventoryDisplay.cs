using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    public GameObject InventoryTileContainer;
    public GameObject InventoryTilePrefab;

    private bool _hasBeenSetup = false;

    private SimInventoryComponent _inventory = null;

    public void ToggleInventory()
    {
        if (!_hasBeenSetup)
        {
            // PORT TO ECS

            //SimPawnComponent playerPawn = PlayerIdHelpers.GetLocalSimPawnComponent();

            //_inventory = playerPawn.GetComponent<SimInventoryComponent>();

            //for (int i = 0; i < _inventory.InventorySize; i++)
            //{
            //    SimItem item = _inventory.GetItem(i);
            //    UIInventorySlot newSlot = Instantiate(InventoryTilePrefab, InventoryTileContainer.transform).GetComponent<UIInventorySlot>();
            //    newSlot.Init(item);
            //}

            _hasBeenSetup = true;
        }
        gameObject.SetActive(!gameObject.activeSelf);
    }

    private void Update()
    {
        if (_hasBeenSetup)
        {
            if (_inventory.InventorySize == InventoryTileContainer.transform.childCount)
            {
                return;
            }
            else if (_inventory.InventorySize < InventoryTileContainer.transform.childCount)
            {
                for (int i = 0; i < InventoryTileContainer.transform.childCount - _inventory.InventorySize; ++i) 
                {
                    Destroy(InventoryTileContainer.transform.GetChild(i).gameObject);
                }
            }
            else if (_inventory.InventorySize > InventoryTileContainer.transform.childCount)
            {
                for (int i = InventoryTileContainer.transform.childCount; i <= _inventory.InventorySize - InventoryTileContainer.transform.childCount; ++i)
                {
                    SimItem item = _inventory.GetItem(i);
                    UIInventorySlot newSlot = Instantiate(InventoryTilePrefab, InventoryTileContainer.transform).GetComponent<UIInventorySlot>();
                    newSlot.Init(item);
                }
            }
        }
    }
}
