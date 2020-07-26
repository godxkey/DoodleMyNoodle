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
            DynamicBuffer<InventoryItemReference> items = SimWorld.GetBufferReadOnly<InventoryItemReference>(_lastInventoryEntity);

            if (items.Length != _currentItemSlots.Count)
            {
                ShowAndSetupDisplay(_lastInventoryEntity);
                return;
            }
        }
    }

    public void ShowAndSetupDisplay(Entity inventoryEntity)
    {
        _lastInventoryEntity = inventoryEntity;

        Clear();

        DynamicBuffer<InventoryItemReference> items = SimWorld.GetBufferReadOnly<InventoryItemReference>(inventoryEntity);

        if (items.Length < 1)
        {
            return;
        }

        fix3 bundleEntityPosition = SimWorld.GetComponentData<FixTranslation>(inventoryEntity).Value;

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
                itemSlot.UpdateCurrentItemSlot(itemInfo, null, inventoryEntity);

                // Spawn Take Button
                GameObject newTakeButton = Instantiate(_takeButtonPrefab, _takeButtonContainer);
                Button button = newTakeButton.GetComponent<Button>();
                _currentTakeButtons.Add(button);
                
                // Get position of the entity of sim input
                int2 bundleTilePosition = new int2() { x = fix.RoundToInt(bundleEntityPosition.x), y = fix.RoundToInt(bundleEntityPosition.y) };
                int itemSlotID = itemSlot.GetItemInfoInSlot().ID.GetSimAssetId().Value;

                button.onClick.AddListener(() =>
                {
                    // when clicking on Take Item, we send a sim input
                    SimPlayerInputEquipItem simInputEquipItem = new SimPlayerInputEquipItem(itemIDComponent.Value, bundleTilePosition);
                    SimWorld.SubmitInput(simInputEquipItem);

                    for (int j = 0; j < _currentItemSlots.Count; j++)
                    {
                        ItemSlot currentItemSlot = _currentItemSlots[j];

                        if (currentItemSlot.GetItemInfoInSlot().ID.GetSimAssetId().Value == itemSlotID)
                        {
                            // Clear display of itemSlot
                            Destroy(_currentItemSlots[j].gameObject);
                            _currentItemSlots.RemoveAt(j);
                            Destroy(_currentTakeButtons[j].gameObject);
                            _currentTakeButtons.RemoveAt(j);

                            if (_currentItemSlots.Count < 1)
                            {
                                CloseDisplay();
                                _lastInventoryEntity = Entity.Null;
                            }
                        }
                    }
                });
            }
        }

        _bundleDisplayContainer.SetActive(true);
    }

    public void CloseDisplay()
    {
        _bundleDisplayContainer.SetActive(false);
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