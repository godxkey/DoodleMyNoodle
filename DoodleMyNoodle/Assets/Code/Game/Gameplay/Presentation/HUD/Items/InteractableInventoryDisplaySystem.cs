using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

public class InteractableInventoryDisplaySystem : GamePresentationSystem<InteractableInventoryDisplaySystem>
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
            if (SimWorld.Exists(_lastInventoryEntity)
                && SimWorld.TryGetBufferReadOnly(_lastInventoryEntity, out DynamicBuffer<InventoryItemReference> items))
            {
                if (items.Length < 1)
                {
                    CloseDisplay();
                    return;
                }

                // Showing Bundle Display
                _bundleDisplayContainer.SetActive(true);

                // Get position of the entity
                fix3 bundleEntityPosition = SimWorld.GetComponentData<FixTranslation>(_lastInventoryEntity).Value;
                int2 bundleTilePosition = Helpers.GetTile(SimWorld.GetComponentData<FixTranslation>(_lastInventoryEntity));

                int i = 0;
                for (; i < items.Length; i++)
                {
                    InventoryItemReference item = items[i];

                    if (i >= _currentItemSlots.Count)
                    {
                        // - Instantiate item slot
                        GameObject newItemSlot = Instantiate(_itemSlotPrefab, _itemSlotContainer);
                        ItemSlot itemSlot = newItemSlot.GetComponent<ItemSlot>();
                        _currentItemSlots.Add(itemSlot);

                        // - Instantiate take button
                        GameObject newTakeButton = Instantiate(_takeButtonPrefab, _takeButtonContainer);
                        Button button = newTakeButton.GetComponent<Button>();
                        _currentTakeButtons.Add(button);

                        // - Add onClick listener on button
                        int itemIndex = i;
                        button.onClick.AddListener(() =>
                        {
                            // when clicking on Take Item, we send a sim input
                            SimPlayerInputEquipItem simInputEquipItem = new SimPlayerInputEquipItem(itemIndex, bundleTilePosition);
                            SimWorld.SubmitInput(simInputEquipItem);
                        });
                    }

                    // Update Item Slot Stacks
                    if (SimWorld.TryGetComponentData(item.ItemEntity, out SimAssetId itemIDComponent))
                    {
                        ItemVisualInfo itemInfo = ItemVisualInfoBank.Instance.GetItemInfoFromID(itemIDComponent);

                        if (SimWorld.TryGetComponentData(item.ItemEntity, out ItemStackableData itemStackableData))
                        {
                            _currentItemSlots[i].UpdateCurrentItemSlot(itemInfo, null, null, _lastInventoryEntity, itemStackableData.Value);
                        }
                        else
                        {
                            _currentItemSlots[i].UpdateCurrentItemSlot(itemInfo, null, null, _lastInventoryEntity);
                        }
                    }
                }

                for (int r = _currentItemSlots.Count - 1; r >= i; r--)
                {
                    Destroy(_currentItemSlots[r].gameObject);
                    _currentItemSlots.RemoveAt(r);

                    Destroy(_currentTakeButtons[r].gameObject);
                    _currentTakeButtons.RemoveAt(r);
                }
            }
            else
            {
                CloseDisplay();
            }
        }
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

    public bool IsOpen()
    {
        return _bundleDisplayContainer.activeSelf;
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