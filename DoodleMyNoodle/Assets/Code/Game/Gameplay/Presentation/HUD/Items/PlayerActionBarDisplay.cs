

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using static Unity.Mathematics.math;

public class PlayerActionBarDisplay : GamePresentationBehaviour
{
    [SerializeField] private GridLayoutGroup _gridLayoutGroup;
    [SerializeField] private int _maxCollumns = 10;
    [SerializeField] private Image _background;
    [SerializeField] private Image _blockedDisplay;
    [SerializeField] private List<PlayerActionBarSlotInfo> _inventorySlotShortcuts = new List<PlayerActionBarSlotInfo>();
    [SerializeField] private Transform _slotsContainer;
    [SerializeField] private PlayerActionBarSlot _inventorySlotPrefab;

    private List<PlayerActionBarSlot> _slotVisuals = new List<PlayerActionBarSlot>();

    public override void OnGameAwake()
    {
        base.OnGameAwake();

        _slotsContainer.GetComponentsInChildren(_slotVisuals);
    }

    protected override void OnGamePresentationUpdate()
    {
        if (Cache.LocalPawn != Entity.Null && Cache.LocalController != Entity.Null)
        {
            UpdateInventorySlots();
        }
    }

    private void UpdateInventorySlots()
    {
        Action<int> onItemPrimaryActionUsedCallback = null;
        Action<int> onItemSecondaryActionUsedCallback = null;

        if (!CommonReads.CanTeamPlay(SimWorld, SimWorld.GetComponentData<Team>(Cache.LocalController)))
        {
            TileHighlightManager.Instance.InterruptTileSelectionProcess();
            _blockedDisplay.gameObject.SetActive(true);
        }
        else
        {
            onItemPrimaryActionUsedCallback = OnIntentionToUsePrimaryActionOnItem;
            onItemSecondaryActionUsedCallback = OnIntentionToUseSecondaryActionOnItem;
            _blockedDisplay.gameObject.SetActive(false);

            VerifyButtonInputForSlots();
        }

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
                    ItemVisualInfo itemInfo = ItemVisualInfoBank.Instance.GetItemInfoFromID(itemAssetId);

                    _slotVisuals[i].UpdateCurrentInventorySlot(itemInfo,
                                                               i,
                                                               GetSlotShotcut(i),
                                                               onItemPrimaryActionUsedCallback,
                                                               onItemSecondaryActionUsedCallback,
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

                        var itemAction = GameActionBank.GetAction(itemActionId);
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
        if (SimWorld.TryGetBufferReadOnly(Cache.LocalPawn, out DynamicBuffer<InventoryItemReference> inventory))
        {
            if (inventory.Length > ItemIndex && ItemIndex > -1)
            {
                InventoryItemReference item = inventory[ItemIndex];
                if (SimWorld.TryGetComponentData(item.ItemEntity, out SimAssetId itemIDComponent))
                {
                    Entity itemEntity = item.ItemEntity;

                    SimWorld.TryGetComponentData(itemEntity, out GameActionId actionId);
                    GameAction itemGameAction = GameActionBank.GetAction(actionId);

                    GameAction.UseContext useContext = new GameAction.UseContext()
                    {
                        InstigatorPawn = Cache.LocalPawn,
                        InstigatorPawnController = Cache.LocalController,
                        Entity = itemEntity
                    };

                    GameAction.UseContract itemUseContract = itemGameAction.GetUseContract(SimWorld, useContext);

                    OnStartUsingNewItem(itemUseContract);

                    QueryUseDataFromPlayer(itemUseContract, () =>
                     {
                         SimPlayerInputUseItem simInput = new SimPlayerInputUseItem(ItemIndex, _currentItemUseData);
                         SimWorld.SubmitInput(simInput);
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

    private void OnStartUsingNewItem(GameAction.UseContract NewItemContact)
    {
        // clean up in case we try to use two items one after the other (cancel feature)
        TileHighlightManager.Instance.InterruptTileSelectionProcess();

        _currentItemUseData = GameAction.UseParameters.Create(new GameAction.ParameterData[NewItemContact.ParameterTypes.Length]);
    }

    private GameAction.UseParameters _currentItemUseData;
    private void QueryUseDataFromPlayer(GameAction.UseContract itemUseContact, Action onComplete, byte DataToExtract = 0)
    {
        if (DataToExtract >= itemUseContact.ParameterTypes.Length)
        {
            onComplete?.Invoke();
            return;
        }

        IdentifyAndGatherDataForParameterDescription(itemUseContact.ParameterTypes[DataToExtract], DataToExtract, () =>
        {
            if (DataToExtract >= itemUseContact.ParameterTypes.Length)
            {
                onComplete?.Invoke();
                return;
            }

            // Little Delay between choices
            this.DelayedCall(0.1f, () => { QueryUseDataFromPlayer(itemUseContact, onComplete, (byte)(DataToExtract + 1)); });
        });
    }

    private void IdentifyAndGatherDataForParameterDescription(GameAction.ParameterDescription parameterDescription, byte index, Action OnComplete)
    {
        // SELECT A SINGLE TILE
        if (parameterDescription is GameActionParameterTile.Description TileDescription)
        {
            if (TileDescription != null)
            {
                TileHighlightManager.Instance.AskForSingleTileSelectionAroundPlayer(TileDescription, (GameActionParameterTile.Data TileSelectedData) =>
                {
                    TileSelectedData.ParamIndex = index;
                    _currentItemUseData.ParameterDatas[index] = TileSelectedData;
                    OnComplete?.Invoke();
                });
                return;
            }
        }

        // SELF TARGETING
        if (parameterDescription is GameActionParameterSelfTarget.Description SelfDescription)
        {
            _currentItemUseData.ParameterDatas[index] = new GameActionParameterSelfTarget.Data(index);
            OnComplete?.Invoke();
            return;
        }

        // other types of Parameter Description here ...
    }
}
