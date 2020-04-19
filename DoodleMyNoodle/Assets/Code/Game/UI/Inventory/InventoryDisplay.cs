using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class InventoryDisplay : GameMonoBehaviour
{
    public List<InventorySlotInfo> InventorySlotShortcuts = new List<InventorySlotInfo>();

    public GameObject SlotsContainer;
    public GameObject InventorySlotPrefab;

    private List<InventorySlot> _inventorySlots = new List<InventorySlot>();

    private Entity _localPawn = Entity.Null;

    public override void OnGameReady()
    {
        base.OnGameReady();

        for (int i = 0; i < InventorySlotShortcuts.Count; i++)
        {
            InventorySlot inventorySlot = Instantiate(InventorySlotPrefab, SlotsContainer.transform).GetComponent<InventorySlot>();
            _inventorySlots.Add(inventorySlot);
        }
    }

    public override void OnGameUpdate()
    {
        UpdateCurrentPlayerPawn();

        int itemIndex = 0;
        if (GameMonoBehaviourHelpers.SimulationWorld.TryGetBuffer(_localPawn, out DynamicBuffer<InventoryItemReference> inventory))
        {
            foreach (InventoryItemReference item in inventory)
            {
                if (GameMonoBehaviourHelpers.SimulationWorld.TryGetComponentData(item.ItemEntity,out SimAssetId itemIDComponent))
                {
                    ItemVisualInfo itemInfo = ItemVisualInfoBank.Instance.GetItemInfoFromID(itemIDComponent);
                    _inventorySlots[itemIndex].UpdateCurrentItemSlot(itemInfo, InventorySlotShortcuts[itemIndex]);
                    itemIndex++;
                }
            }
        }

        // Clear the rest of the inventory slots
        for (int i = _inventorySlots.Count - 1; i > itemIndex; i--)
        {
            _inventorySlots[i].UpdateCurrentItemSlot(null, InventorySlotShortcuts[i]);
        }
    }

    private void UpdateCurrentPlayerPawn()
    {
        if(_localPawn == Entity.Null)
        {
            _localPawn = PlayerIdHelpers.GetLocalSimPawnEntity(GameMonoBehaviourHelpers.SimulationWorld);
        }
    }
}
