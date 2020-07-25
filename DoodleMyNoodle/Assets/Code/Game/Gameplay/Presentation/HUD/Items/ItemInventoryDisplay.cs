using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;

public class ItemInventoryDisplay : GamePresentationSystem<ItemInventoryDisplay>
{
    [SerializeField] private GameObject _itemSlotPrefab;
    [SerializeField] private Transform _itemSlotContainer;

    [SerializeField] private GameObject _takeButtonPrefab;
    [SerializeField] private Transform _takeButtonContainer;

    [SerializeField] private GameObject _bundleDisplayContainer;

    private List<ItemSlot> _currentItemSlots = new List<ItemSlot>();
    private List<Button> _currentTakeButtons = new List<Button>();

    public override bool SystemReady => true;

    protected override void OnGamePresentationUpdate() { }

    public void ShowAndSetupDisplay(Entity inventoryEntity)
    {
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
                ItemVisualInfo itemInfo = ItemVisualInfoBank.Instance.GetItemInfoFromID(itemIDComponent);
                itemSlot.UpdateCurrentItemSlot(itemInfo, null, inventoryEntity);
            }

            // Spawn Take Button
            GameObject newTakeButton = Instantiate(_takeButtonPrefab, _takeButtonContainer);
            Button button = newTakeButton.GetComponent<Button>();
            _currentTakeButtons.Add(button);
            int itemIndex = i;
            button.onClick.AddListener(() =>
            {
                int2 bundleTilePosition = new int2() { x = fix.RoundToInt(bundleEntityPosition.x), y = fix.RoundToInt(bundleEntityPosition.y) };
                int itemPrefabSimAssetID = _currentItemSlots[itemIndex].GetItemInfoInSlot().ID.GetSimAssetId().Value;
                SimPlayerInputEquipItem simInputEquipItem = new SimPlayerInputEquipItem(itemPrefabSimAssetID, bundleTilePosition);
                SimWorld.SubmitInput(simInputEquipItem);
                
                Destroy(_currentItemSlots[itemIndex].gameObject);
                _currentItemSlots.RemoveAt(itemIndex);
                Destroy(_currentTakeButtons[itemIndex].gameObject);
                _currentTakeButtons.RemoveAt(itemIndex);

                if (_currentItemSlots.Count < 1)
                {
                    CloseWindow();
                }
            });
        }

        _bundleDisplayContainer.SetActive(true);
    }

    public void CloseWindow()
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