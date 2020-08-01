using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

public class InteractableInventoryDisplay : GamePresentationSystem<InteractableInventoryDisplay>
{
    [SerializeField] private GameObject _itemSlotPrefab;
    [SerializeField] private Transform _itemSlotContainer;

    [SerializeField] private GameObject _takeButtonPrefab;
    [SerializeField] private Transform _takeButtonContainer;

    [SerializeField] private GameObject _bundleDisplayContainer;

    private List<ItemSlot> _currentItemSlots = new List<ItemSlot>();
    private List<Button> _currentTakeButtons = new List<Button>();

    private Entity _lastInventoryEntity = Entity.Null;

    public override bool SystemReady => true;

    protected override void OnGamePresentationUpdate() 
    {
        if (_lastInventoryEntity != Entity.Null)
        {
            if (SimWorld.Exists(_lastInventoryEntity))
            {
                if(SimWorld.TryGetBufferReadOnly(_lastInventoryEntity, out DynamicBuffer<InventoryItemReference> items)) 
                {
                    if (_currentItemSlots.Count != items.Length)
                    {
                        UpdateDisplay();
                    }
                    else
                    {
                        UpdateItemSlotStacks();
                    }
                }
            }
            else
            {
                CloseDisplay();
            }
        }
    }

    private void UpdateItemSlotStacks()
    {
        DynamicBuffer<InventoryItemReference> items = SimWorld.GetBufferReadOnly<InventoryItemReference>(_lastInventoryEntity);

        if (items.Length < 1)
        {
            return;
        }

        for (int i = 0; i < items.Length; i++)
        {
            InventoryItemReference item = items[i];
            ItemSlot itemSlot = _currentItemSlots[i];

            if (SimWorld.TryGetComponentData(item.ItemEntity, out SimAssetId itemIDComponent))
            {
                // Update Item Slot Stacks
                if (SimWorld.TryGetComponentData(item.ItemEntity, out ItemStackableData itemStackableData))
                {
                    itemSlot.UpdateStacks(itemStackableData.Value);
                }
            }
        }
    }

    private void UpdateDisplay()
    {
        Clear();

        DynamicBuffer<InventoryItemReference> items = SimWorld.GetBufferReadOnly<InventoryItemReference>(_lastInventoryEntity);

        if (items.Length < 1)
        {
            CloseDisplay();
            return;
        }

        fix3 bundleEntityPosition = SimWorld.GetComponentData<FixTranslation>(_lastInventoryEntity).Value;

        for (int i = 0; i < items.Length; i++)
        {
            InventoryItemReference item = items[i];

            // Spawn Item Slot
            GameObject newItemSlot = Instantiate(_itemSlotPrefab, _itemSlotContainer);
            ItemSlot itemSlot = newItemSlot.GetComponent<ItemSlot>();
            _currentItemSlots.Add(itemSlot);
            if (SimWorld.TryGetComponentData(item.ItemEntity, out SimAssetId itemIDComponent))
            {
                // Update Item Slot
                ItemVisualInfo itemInfo = ItemVisualInfoBank.Instance.GetItemInfoFromID(itemIDComponent);
                if (SimWorld.TryGetComponentData(item.ItemEntity, out ItemStackableData itemStackableData))
                {
                    itemSlot.UpdateCurrentItemSlot(itemInfo, null, null, _lastInventoryEntity, itemStackableData.Value);
                }
                else
                {
                    itemSlot.UpdateCurrentItemSlot(itemInfo, null, null, _lastInventoryEntity);
                }

                // Spawn Take Button
                GameObject newTakeButton = Instantiate(_takeButtonPrefab, _takeButtonContainer);
                Button button = newTakeButton.GetComponent<Button>();
                _currentTakeButtons.Add(button);

                // Get position of the entity of sim input
                int2 bundleTilePosition = new int2() { x = fix.RoundToInt(bundleEntityPosition.x), y = fix.RoundToInt(bundleEntityPosition.y) };
                int itemIndex = i;

                button.onClick.AddListener(() =>
                {
                    // when clicking on Take Item, we send a sim input
                    SimPlayerInputEquipItem simInputEquipItem = new SimPlayerInputEquipItem(itemIndex, bundleTilePosition);
                    SimWorld.SubmitInput(simInputEquipItem);
                });
            }
        }

        _bundleDisplayContainer.SetActive(true);
    }

    public void SetupDisplayForInventory(Entity inventoryEntity)
    {
        Clear();

        _lastInventoryEntity = inventoryEntity;
    }

    public void CloseDisplay()
    {
        _bundleDisplayContainer.SetActive(false);
        _lastInventoryEntity = Entity.Null;
    }

    private void Clear()
    {
        for (int i = 0; i < _currentItemSlots.Count; i++)
        {
            Destroy(_currentItemSlots[i].gameObject);
        }

        for (int i = 0; i < _currentTakeButtons.Count; i++)
        {
            Destroy(_currentTakeButtons[i].gameObject);
        }

        _currentItemSlots.Clear();
        _currentTakeButtons.Clear();
    }
}