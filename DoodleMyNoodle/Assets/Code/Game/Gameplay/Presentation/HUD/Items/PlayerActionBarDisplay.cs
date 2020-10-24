

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerActionBarDisplay : GamePresentationSystem<PlayerActionBarDisplay>
{
    [SerializeField] private GridLayoutGroup _gridLayoutGroup;
    [SerializeField] private int _maxCollumns = 10;
    [SerializeField] private Image _background;
    [SerializeField] private Image _blockedDisplay;
    [SerializeField] private List<PlayerActionBarSlotInfo> _inventorySlotShortcuts = new List<PlayerActionBarSlotInfo>();
    [SerializeField] private Transform _slotsContainer;
    [SerializeField] private PlayerActionBarSlot _inventorySlotPrefab;

    private List<PlayerActionBarSlot> _inventorySlots = new List<PlayerActionBarSlot>();

    private bool _canBeInteractedWith = true;

    public override void OnGameAwake()
    {
        base.OnGameAwake();

        _slotsContainer.GetComponentsInChildren(_inventorySlots);
        _canBeInteractedWith = true;
    }

    protected override void OnGamePresentationUpdate()
    {
        if (Cache.LocalPawn != Entity.Null && Cache.LocalController != Entity.Null)
        {
            UpdateActionBarSlots(OnIntentionToUsePrimaryActionOnItem, OnIntentionToUseSecondaryActionOnItem);

            VerifyButtonInputForSlots();
        }
    }

    public void BlockInteraction()
    {
        _blockedDisplay.gameObject.SetActive(true);
        _canBeInteractedWith = false;
    }

    public void EnableInteraction()
    {
        _blockedDisplay.gameObject.SetActive(false);
        _canBeInteractedWith = true;
    }

    private void UpdateActionBarSlots(Action<int> primaryUseCallback, Action<int> secondaryUseCallback)
    {
        int itemIndex = 0;
        if (SimWorld.TryGetBufferReadOnly(Cache.LocalPawn, out DynamicBuffer<InventoryItemReference> inventory))
        {
            // Ajust Slot amount accordiwdng to inventory max size
            InventoryCapacity inventoryCapacity = SimWorld.GetComponentData<InventoryCapacity>(Cache.LocalPawn);

            _gridLayoutGroup.constraintCount = Mathf.Min(_maxCollumns, inventoryCapacity);

            UIUtility.ResizeGameObjectList(_inventorySlots, Mathf.Min(inventoryCapacity, Mathf.Max(_maxCollumns, inventory.Length)), _inventorySlotPrefab, _slotsContainer);

            foreach (InventoryItemReference item in inventory)
            {
                if (SimWorld.TryGetComponentData(item.ItemEntity, out SimAssetId itemIDComponent))
                {
                    ItemVisualInfo itemInfo = ItemVisualInfoBank.Instance.GetItemInfoFromID(itemIDComponent);

                    GameAction.UseContext context = new GameAction.UseContext()
                    {
                        InstigatorPawn = Cache.LocalPawn,
                        InstigatorPawnController = Cache.LocalController,
                        Entity = item.ItemEntity
                    };

                    int stacks = -1;
                    if (SimWorld.TryGetComponentData(item.ItemEntity, out ItemStackableData itemStackableData))
                    {
                        stacks = itemStackableData.Value;
                    }

                    _inventorySlots[itemIndex].UpdateCurrentInventorySlot(itemInfo,
                                                                          itemIndex,
                                                                          GetSlotShotcut(itemIndex),
                                                                          primaryUseCallback,
                                                                          secondaryUseCallback,
                                                                          stacks);

                    if (!GameActionBank.GetAction(SimWorld.GetComponentData<GameActionId>(item.ItemEntity)).CanBeUsedInContext(SimWorld, context))
                    {
                        _inventorySlots[itemIndex].UpdateDisplayAsUnavailable(item.ItemEntity);
                    }

                    itemIndex++;
                }
            }

            // Empty remaining inventory slots
            for (int i = _inventorySlots.Count - 1; i >= itemIndex; i--)
            {
                _inventorySlots[i].UpdateCurrentInventorySlot(null, i, GetSlotShotcut(i), null, null);
            }
        }
    }

    private PlayerActionBarSlotInfo GetSlotShotcut(int itemIndex)
    {
        return itemIndex < _inventorySlotShortcuts.Count ? _inventorySlotShortcuts[itemIndex] : PlayerActionBarSlotInfo.Default;
    }

    private void VerifyButtonInputForSlots()
    {
        // We permit one input per frame
        for (int i = 0; i < _inventorySlotShortcuts.Count; i++)
        {
            if (Input.GetKeyDown(_inventorySlotShortcuts[i].InputShortcut))
            {
                if (_inventorySlots.Count > i)
                {
                    _inventorySlots[i].PrimaryUseItemSlot();
                }
                return;
            }
        }
    }

    private void OnIntentionToUsePrimaryActionOnItem(int ItemIndex)
    {
        if (!_canBeInteractedWith)
            return;

        if (SimWorld.TryGetBufferReadOnly(Cache.LocalPawn, out DynamicBuffer<InventoryItemReference> inventory))
        {
            if (inventory.Length > ItemIndex && ItemIndex > -1)
            {
                InventoryItemReference item = inventory[ItemIndex];
                if (SimWorld.TryGetComponentData(item.ItemEntity, out SimAssetId itemIDComponent))
                {
                    Entity itemEntity = item.ItemEntity;

                    UIStateMachine.Instance.TransitionTo(UIState.StateTypes.ParameterSelection, itemEntity, ItemIndex);
                }
            }
        }
    }

    private void OnIntentionToUseSecondaryActionOnItem(int ItemIndex)
    {
        if (!_canBeInteractedWith)
            return;

        if (SimWorld.TryGetBufferReadOnly(Cache.LocalPawn, out DynamicBuffer<InventoryItemReference> inventory))
        {
            if (inventory.Length > ItemIndex && ItemIndex > -1)
            {
                int currentItemIndex = ItemIndex;
                ItemContextMenuDisplaySystem.Instance.ActivateContextMenuDisplay((int? actionIndex) =>
                {
                    if (actionIndex == 0)
                    {
                        SimPlayerInputDropItem simInput = new SimPlayerInputDropItem(currentItemIndex);
                        SimWorld.SubmitInput(simInput);
                    }
                }, "Drop");
            }
        }
    }
}
