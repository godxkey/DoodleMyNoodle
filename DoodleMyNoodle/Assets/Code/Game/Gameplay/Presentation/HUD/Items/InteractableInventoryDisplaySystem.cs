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
    private List<(Entity item, int stack)> _itemData = new List<(Entity item, int stack)>();

    private Entity _lastInventoryEntity = Entity.Null;

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

        if (!Cache.LocalPawnAlive)
            return;

        if (_lastInventoryEntity == Entity.Null)
            return;

        if (!SimWorld.Exists(_lastInventoryEntity))
            return;

        if (!SimWorld.TryGetBufferReadOnly(_lastInventoryEntity, out DynamicBuffer<InventoryItemReference> items))
            return;

        fix2 pos = SimWorld.GetComponent<FixTranslation>(_lastInventoryEntity);

        if (fixMath.distancemanhattan(pos, Cache.LocalPawnPosition) > SimulationGameConstants.InteractibleMaxDistanceManhattan)
            return;

        foreach (var item in items)
        {
            _itemData.Add((item.ItemEntity, item.Stacks));
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
                Entity item = _itemData[i].item;
                ItemSlot slot = _currentItemSlots[i];

                if (SimWorld.TryGetComponent(item, out SimAssetId itemIDComponent))
                {
                    int itemIndex = i;
                    ItemAuth itemAuth = PresentationHelpers.FindItemAuth(itemIDComponent);

                    Action onClick = () =>
                    {
                        // when clicking on Take Item, we send a sim input
                        SimPlayerInputEquipItem simInputEquipItem = new SimPlayerInputEquipItem(itemIndex, _lastInventoryEntity);
                        SimWorld.SubmitInput(simInputEquipItem);
                    };

                    int stacks = _itemData[i].stack;
                    if (stacks == 1 && !SimWorld.GetComponent<StackableFlag>(item))
                        stacks = -1;

                    _currentItemSlots[i].UpdateCurrentItemSlot(itemAuth, onClick, null, _lastInventoryEntity, stacks);
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