

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

    private struct DisplayedItemData
    {
        public InventoryItemReference ItemRef;
        public int Index;
        public ItemAuth ItemAuth;
    }

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
            List<DisplayedItemData> displayedInventory = ListPool<DisplayedItemData>.Take();

            // gather all items to display
            for (int i = 0; i < inventory.Length; i++)
            {
                Entity item = inventory[i].ItemEntity;

                if (SimWorld.TryGetComponent(item, out SimAssetId itemAssetId))
                {
                    ItemAuth itemGameActionAuth = PresentationHelpers.FindItemAuth(itemAssetId);
                    if (itemGameActionAuth != null && !itemGameActionAuth.HideInInventory)
                    {
                        displayedInventory.Add(new DisplayedItemData()
                        {
                            ItemAuth = itemGameActionAuth,
                            ItemRef = inventory[i],
                            Index = i
                        });
                    }
                }
            }

            // Ajust Slot amount accordiwdng to inventory max size
            InventoryCapacity inventoryCapacity = SimWorld.GetComponent<InventoryCapacity>(Cache.LocalPawn);

            _gridLayoutGroup.constraintCount = min(_maxCollumns, inventoryCapacity);

            PresentationHelpers.ResizeGameObjectList(_slotVisuals, max(min(_maxCollumns, inventoryCapacity), displayedInventory.Count), _inventorySlotPrefab, _slotsContainer);

            for (int i = 0; i < _slotVisuals.Count; i++)
            {
                if (i < displayedInventory.Count)
                {
                    Entity item = displayedInventory[i].ItemRef.ItemEntity;
                    int stacks = displayedInventory[i].ItemRef.Stacks;

                    if (stacks == 1 && !SimWorld.GetComponent<StackableFlag>(item))
                        stacks = -1; // used in display to hide stacks

                    _slotVisuals[i].UpdateCurrentInventorySlot(displayedInventory[i].ItemAuth,
                                                               displayedInventory[i].Index,
                                                               GetSlotShotcut(i),
                                                               OnIntentionToUsePrimaryActionOnItem,
                                                               OnIntentionToUseSecondaryActionOnItem,
                                                               stacks);

                    bool canBeUsed = false;
                    if (SimWorld.TryGetComponent(item, out ItemAction itemAction))
                    {
                        if (SimWorld.TryGetComponent(itemAction.ActionPrefab, out GameActionId itemActionId))
                        {
                            GameAction action = GameActionBank.GetAction(itemActionId);
                            canBeUsed = action != null && action.CanBeUsedInContext(SimWorld, CommonReads.GetActionContext(SimWorld, item, itemAction.ActionPrefab));
                        }
                    }

                    if (!displayedInventory[i].ItemAuth.UsableInOthersTurn && !Cache.DEPRECATED_CanLocalPlayerPlay)
                    {
                        canBeUsed = false;
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

            ListPool<DisplayedItemData>.Release(displayedInventory);
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

                    if (SimWorld.TryGetComponent(itemEntity, out ItemAction itemAction))
                    {
                        UIStateMachine.Instance.TransitionTo(UIStateType.ParameterSelection, new ParameterSelectionState.InputParam()
                        {
                            ActionInstigator = itemEntity,
                            ActionPrefab = itemAction.ActionPrefab,
                            IsItem = true,
                            ItemIndex = ItemIndex
                        });
                    }
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
