

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngineX;
using static Unity.Mathematics.math;

public class WidgetControllerPlayerActionBar : GamePresentationSystem<WidgetControllerPlayerActionBar>
{
    private class WidgetDataSlotData : IWidgetDataButton, IWidgetDataItem, IWidgetDataPlayerActionBarItem
    {
        public ButtonData ButtonData { get; set; } = new ButtonData();
        public ItemAuth ItemAuth { get; set; }
        public int ItemStacks { get; set; }
        public bool ShowItemStacks { get; set; }
        public KeyCode KeyShortcut { get; set; }
        public float CooldownTime { get; set; }
        public bool IsBeingUsed { get; set; }

        // meta data
        public int ItemIndex;
    }

    [SerializeField] private GridLayoutGroup _gridLayoutGroup;
    [SerializeField] private int _maxCollumns = 10;
    [SerializeField] private Image _blockedDisplay;
    [SerializeField] private List<KeyCode> _inventorySlotShortcuts = new List<KeyCode>();
    [SerializeField] private Widget _slotContainer;
    [SerializeField] private CanvasGroup _canvasGroup;

    private List<WidgetDataSlotData> _slotWidgetDatas = new List<WidgetDataSlotData>();
    private Widget _currentSlotInUse = null;

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
    }

    public override void PresentationUpdate()
    {
        if (Cache.LocalPawn != Entity.Null && Cache.LocalController != Entity.Null)
        {
            UpdateInventorySlots();
        }
    }

    private void OnInteractableChange(bool interactable)
    {
        _canvasGroup.interactable = interactable;
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
            using var _ = ListPool<DisplayedItemData>.Take(out var displayedInventory);

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

            int displayedSlots = max(min(_maxCollumns, inventoryCapacity), displayedInventory.Count);
            int newEntryCount = _slotWidgetDatas.Resize(displayedSlots);
            if (newEntryCount > 0)
            {
                for (int i = _slotWidgetDatas.Count - newEntryCount; i < _slotWidgetDatas.Count; i++)
                {
                    var widgetData = _slotWidgetDatas[i];
                    widgetData.ButtonData.Clicked = OnSlotClicked;
                    widgetData.ButtonData.Pressed = OnSlotPressed;
                    widgetData.ButtonData.Released = OnSlotReleased;
                }
            }

            for (int i = 0; i < _slotWidgetDatas.Count; i++)
            {
                var widgetData = _slotWidgetDatas[i];

                if (i < displayedInventory.Count)
                {
                    Entity item = displayedInventory[i].ItemRef.ItemEntity;
                    int charges = -1;
                    if (SimWorld.TryGetComponent<ItemCharges>(item, out var itemCharges))
                        charges = itemCharges;

                    bool itemCanBeUsed = CommonReads.CanUseItem(SimWorld, Cache.LocalPawn, item);
                    widgetData.ItemIndex = i;
                    widgetData.ItemStacks = charges;
                    widgetData.ItemAuth = displayedInventory[i].ItemAuth;
                    widgetData.ShowItemStacks = charges != -1;
                    widgetData.ButtonData.Interactable = itemCanBeUsed;
                    widgetData.KeyShortcut = i < _inventorySlotShortcuts.Count ? _inventorySlotShortcuts[i] : KeyCode.None;
                    widgetData.ButtonData.KeyboardShortcut = widgetData.KeyShortcut;
                    if (!itemCanBeUsed && SimWorld.TryGetComponent(item, out ItemCooldownTimeCounter timerCounter) && timerCounter.Value != 0)
                    {
                        widgetData.CooldownTime = (float)timerCounter.Value;
                    }
                    else
                    {
                        widgetData.CooldownTime = 0;
                    }
                }
                else
                {
                    widgetData.KeyShortcut = KeyCode.None;
                    widgetData.ItemStacks = 0;
                    widgetData.ItemAuth = null;
                    widgetData.ShowItemStacks = false;
                    widgetData.ButtonData.Interactable = false;
                }
            }

            _slotContainer.SetData(_slotWidgetDatas);
        }
    }

    private void OnSlotPressed(Widget slotWidget)
    {
        var slotData = slotWidget.GetData<WidgetDataSlotData>();
        bool pressedByKey = Input.GetKeyDown(slotData.KeyShortcut);
        if (pressedByKey)
        {
            BeginItemUse(slotWidget);
        }
    }

    private void OnSlotReleased(Widget slotWidget)
    {
        //if (slotWidget == _currentSlotInUse)
        //{
        //    var slotData = slotWidget.GetData<WidgetDataSlotData>();
        //    bool releasedByKey = Input.GetKeyUp(slotData.KeyShortcut);
        //    if (releasedByKey)
        //    {
        //        CompleteItemUse();
        //    }
        //}
    }

    private void OnSlotClicked(Widget slotWidget)
    {
        var slotData = slotWidget.GetData<WidgetDataSlotData>();
        bool clickedByKey = Input.GetKeyUp(slotData.KeyShortcut);
        if (!clickedByKey)
        {
            BeginItemUse(slotWidget);
        }
    }

    private void BeginItemUse(Widget slotWidget)
    {
        if (_currentSlotInUse != null)
        {
            var data = _currentSlotInUse.GetData<WidgetDataSlotData>();
            data.IsBeingUsed = false;
            _currentSlotInUse.SetData(data);
        }

        _currentSlotInUse = slotWidget;

        if (_currentSlotInUse != null)
        {
            var data = _currentSlotInUse.GetData<WidgetDataSlotData>();
            data.IsBeingUsed = true;
            _currentSlotInUse.SetData(data);

            var slotData = _currentSlotInUse.GetData<WidgetDataSlotData>();
            var itemIndex = slotData.ItemIndex;

            if (SimWorld.TryGetBufferReadOnly(Cache.LocalPawn, out DynamicBuffer<InventoryItemReference> inventory))
            {
                if (inventory.Length > itemIndex && itemIndex > -1)
                {
                    InventoryItemReference item = inventory[itemIndex];

                    if (SimWorld.Exists(item.ItemEntity))
                    {
                        Entity itemEntity = item.ItemEntity;

                        UIStateMachine.Instance.TransitionTo(UIStateType.SpellCasting, new SpellCastingState.InputParam()
                        {
                            PressedKey = Input.GetKeyDown(slotData.KeyShortcut) ? slotData.KeyShortcut : KeyCode.None,
                            ItemEntity = itemEntity,
                            IsItem = true,
                            ItemIndex = itemIndex,
                            OnFinishOrCancelCallback = () => CompleteItemUse(slotWidget)
                        });
                    }
                }
            }
        }
    }

    private void CompleteItemUse(Widget slotWidget)
    {
        if (slotWidget == _currentSlotInUse)
            BeginItemUse(null); // stops the current item use
    }

    // disabled for now

    //private void OnIntentionToUseSecondaryActionOnItem(Widget slot)
    //{
    //    var slotData = slot.GetData<WidgetDataSlotData>();
    //    var itemIndex = slotData.ItemIndex;

    //    if (SimWorld.TryGetBufferReadOnly(Cache.LocalPawn, out DynamicBuffer<InventoryItemReference> inventory))
    //    {
    //        if (inventory.Length > itemIndex && itemIndex > -1)
    //        {
    //            int currentItemIndex = itemIndex;
    //            ItemContextMenuDisplaySystem.Instance.ActivateContextMenuDisplay((int? actionIndex) =>
    //            {
    //                if (actionIndex == 0)
    //                {
    //                    SimPlayerInputDropItem simInput = new SimPlayerInputDropItem(currentItemIndex);
    //                    SimWorld.SubmitInput(simInput);
    //                }
    //            }, "Drop");
    //        }
    //    }
    //}
}
