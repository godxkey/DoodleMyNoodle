using System;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngineX;
using CCC.Fix2D;

public class InteractableInventoryDisplaySystem : GamePresentationSystem<InteractableInventoryDisplaySystem>
{
    [SerializeField] private ItemSlot _itemSlotPrefab;
    [SerializeField] private Transform _itemSlotContainer;
    [SerializeField] private GameObject _displayContainer;
    [SerializeField] private Button _closeButton;

    private List<ItemSlot> _currentItemSlots = new List<ItemSlot>();
    private List<Entity> _itemData = new List<Entity>();

    private Entity _lastInventoryEntity = Entity.Null;
    private int2 _lastInventoryTile;

    protected override void Awake()
    {
        base.Awake();

        CloseDisplay();
        _itemSlotContainer.GetComponentsInChildren(_currentItemSlots);
        _closeButton.onClick.AddListener(CloseDisplay);
    }

    protected override void OnGamePresentationUpdate()
    {
        UpdateData();
        UpdateView();
    }

    private void UpdateData()
    {
        _itemData.Clear();
        if (_lastInventoryEntity != Entity.Null)
        {
            if (SimWorld.Exists(_lastInventoryEntity) &&
                SimWorld.TryGetBufferReadOnly(_lastInventoryEntity, out DynamicBuffer<InventoryItemReference> items))
            {
                _lastInventoryTile = Helpers.GetTile(SimWorld.GetComponentData<FixTranslation>(_lastInventoryEntity));

                foreach (var item in items)
                {
                    _itemData.Add(item);
                }
            }
        }
    }

    private void UpdateView()
    {
        if (_itemData.Count > 0)
        {
            _displayContainer.SetActive(true);

            UIUtility.ResizeGameObjectList(_currentItemSlots, _itemData.Count, _itemSlotPrefab, _itemSlotContainer);

            for (int i = 0; i < _itemData.Count; i++)
            {
                Entity item = _itemData[i];
                ItemSlot slot = _currentItemSlots[i];

                if (SimWorld.TryGetComponentData(item, out SimAssetId itemIDComponent))
                {
                    int itemIndex = i;
                    ItemAuth itemAuth = ItemInfoBank.Instance.GetItemAuthFromID(itemIDComponent);

                    Action onClick = () =>
                    {
                        // when clicking on Take Item, we send a sim input
                        SimPlayerInputEquipItem simInputEquipItem = new SimPlayerInputEquipItem(itemIndex, _lastInventoryTile);
                        SimWorld.SubmitInput(simInputEquipItem);
                    };

                    // Update Item Slot Stacks
                    if (SimWorld.TryGetComponentData(item, out ItemStackableData itemStackableData))
                    {
                        _currentItemSlots[i].UpdateCurrentItemSlot(itemAuth, onClick, null, _lastInventoryEntity, itemStackableData.Value);
                    }
                    else
                    {
                        _currentItemSlots[i].UpdateCurrentItemSlot(itemAuth, onClick, null, _lastInventoryEntity);
                    }
                }
                else
                {
                    _currentItemSlots[i].UpdateCurrentItemSlot(null, null, null, Entity.Null);
                }
            }
        }
        else
        {
            CloseDisplay();
        }
    }

    public void SetupDisplayForInventory(Entity inventoryEntity)
    {
        _lastInventoryEntity = inventoryEntity;
    }

    public void CloseDisplay()
    {
        _displayContainer.SetActive(false);
        _lastInventoryEntity = Entity.Null;
    }

    public bool IsOpen()
    {
        return _displayContainer.activeSelf;
    }
}