

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngineX;
using static Unity.Mathematics.math;

public class PlayerActionBarDisplay : GamePresentationSystem<PlayerActionBarDisplay>
{
    [SerializeField] private GridLayoutGroup _gridLayoutGroup;
    [SerializeField] private int _maxCollumns = 10;
    [SerializeField] private Image _background;
    [SerializeField] private Image _blockedDisplay;
    [SerializeField] private List<PlayerActionBarSlotInfo> _inventorySlotShortcuts = new List<PlayerActionBarSlotInfo>();
    [SerializeField] private Transform _slotsContainer;
    [SerializeField] private PlayerActionBarSlot _inventorySlotPrefab;

    private List<PlayerActionBarSlot> _slotVisuals = new List<PlayerActionBarSlot>();

    private DisablableValue _interactible = new DisablableValue();

    public override void OnGameAwake()
    {
        base.OnGameAwake();

        _interactible.ValueChanged += OnInteractableChange;
        _slotsContainer.GetComponentsInChildren(_slotVisuals);
    }

    protected override void OnGamePresentationUpdate()
    {
        if (Cache.LocalPawn != Entity.Null && Cache.LocalController != Entity.Null)
        {
            UpdateInventorySlots();

            VerifyButtonInputForSlots();
        }
    }

    private void OnInteractableChange(bool interactable)
    {
        _blockedDisplay.gameObject.SetActive(!interactable);
    }

    public void DisableInteraction(string cause)
    {
        _interactible.AddDisable(cause);
    }

    public void UndoDisableInteraction(string cause)
    {
        _interactible.RemoveDisable(cause);
    }

    private void UpdateInventorySlots()
    {
        if (SimWorld.TryGetBufferReadOnly(Cache.LocalPawn, out DynamicBuffer<InventoryItemReference> inventory))
        {
            // Ajust Slot amount accordiwdng to inventory max size
            InventoryCapacity inventoryCapacity = SimWorld.GetComponentData<InventoryCapacity>(Cache.LocalPawn);

            _gridLayoutGroup.constraintCount = min(_maxCollumns, inventoryCapacity);

            UIUtility.ResizeGameObjectList(_slotVisuals, max(min(_maxCollumns, inventoryCapacity), inventory.Length), _inventorySlotPrefab, _slotsContainer);

            for (int i = 0; i < _slotVisuals.Count; i++)
            {
                if (i < inventory.Length)
                {
                    Entity item = inventory[i];

                    int stacks = -1;
                    if (SimWorld.TryGetComponentData(item, out ItemStackableData itemStackableData))
                    {
                        stacks = itemStackableData.Value;
                    }

                    SimWorld.TryGetComponentData(inventory[i], out SimAssetId itemAssetId);
                    ItemAuth itemAuth = ItemInfoBank.Instance.GetItemAuthFromID(itemAssetId);

                    _slotVisuals[i].UpdateCurrentInventorySlot(itemAuth,
                                                               i,
                                                               GetSlotShotcut(i),
                                                               OnIntentionToUsePrimaryActionOnItem,
                                                               OnIntentionToUseSecondaryActionOnItem,
                                                               stacks);

                    bool canBeUsed = false;
                    if (SimWorld.TryGetComponentData(item, out GameActionId itemActionId))
                    {
                        GameAction.UseContext useContext = new GameAction.UseContext()
                        {
                            InstigatorPawn = Cache.LocalPawn,
                            InstigatorPawnController = Cache.LocalController,
                            Entity = item
                        };

                        GameAction itemAction = GameActionBank.GetAction(itemActionId);
                        canBeUsed = itemAction != null && itemAction.CanBeUsedInContext(SimWorld, useContext);
                    }


                    if (!canBeUsed)
                    {
                        _slotVisuals[i].UpdateDisplayAsUnavailable(item);
                    }
                }
                else
                {
                    _slotVisuals[i].UpdateCurrentInventorySlot(null, i, GetSlotShotcut(i), null, null);
                }
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
                if (_slotVisuals.Count > i)
                {
                    _slotVisuals[i].PrimaryUseItemSlot();
                }
                return;
            }
        }
    }

    private void OnIntentionToUsePrimaryActionOnItem(int ItemIndex)
    {
        if (!_interactible)
            return;

        if (SimWorld.TryGetBufferReadOnly(Cache.LocalPawn, out DynamicBuffer<InventoryItemReference> inventory))
        {
            if (inventory.Length > ItemIndex && ItemIndex > -1)
            {
                InventoryItemReference item = inventory[ItemIndex];

                if (SimWorld.Exists(item.ItemEntity))
                {
                    Entity itemEntity = item.ItemEntity;

                    UIStateMachine.Instance.TransitionTo(UIStateType.ParameterSelection, new ParameterSelectionState.InputParam()
                    {
                        ObjectEntity = itemEntity,
                        IsItem = true,
                        ItemIndex = ItemIndex
                    });
                }
            }
        }
    }

    private void OnIntentionToUseSecondaryActionOnItem(int ItemIndex)
    {
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
