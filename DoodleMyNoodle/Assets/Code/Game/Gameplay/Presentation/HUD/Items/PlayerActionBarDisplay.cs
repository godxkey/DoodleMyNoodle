

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class PlayerActionBarDisplay : GamePresentationBehaviour
{
    [SerializeField] private GridLayoutGroup _gridLayoutGroup;
    [SerializeField] private int _maxCollumns = 10;
    [SerializeField] private Image _background;
    [SerializeField] private Image _blockedDisplay;
    [SerializeField] private List<PlayerActionBarSlotInfo> _inventorySlotShortcuts = new List<PlayerActionBarSlotInfo>();
    [SerializeField] private Transform _slotsContainer;
    [SerializeField] private PlayerActionBarSlot _inventorySlotPrefab;

    private List<PlayerActionBarSlot> _inventorySlots = new List<PlayerActionBarSlot>();

    public override void OnGameAwake()
    {
        base.OnGameAwake();

        _slotsContainer.GetComponentsInChildren(_inventorySlots);
    }

    protected override void OnGamePresentationUpdate()
    {
        if (SimWorldCache.LocalPawn != Entity.Null && SimWorldCache.LocalController != Entity.Null)
        {
            UpdateInventorySlots();
        }
    }

    private void UpdateInventorySlots()
    {
        Action<int> onItemPrimaryActionUsedCallback = null;
        Action<int> onItemSecondaryActionUsedCallback = null;

        if (!CommonReads.CanTeamPlay(SimWorld, SimWorld.GetComponentData<Team>(SimWorldCache.LocalController)))
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

        int itemIndex = 0;
        if (SimWorld.TryGetBufferReadOnly(SimWorldCache.LocalPawn, out DynamicBuffer<InventoryItemReference> inventory))
        {
            // Ajust Slot amount accordiwdng to inventory max size
            InventoryCapacity inventoryCapacity = SimWorld.GetComponentData<InventoryCapacity>(SimWorldCache.LocalPawn);

            _gridLayoutGroup.constraintCount = Mathf.Min(_maxCollumns, inventoryCapacity);

            UIUtility.ResizeGameObjectList(_inventorySlots, Mathf.Min(inventoryCapacity, Mathf.Max(_maxCollumns, inventory.Length)), _inventorySlotPrefab, _slotsContainer);

            foreach (InventoryItemReference item in inventory)
            {
                if (SimWorld.TryGetComponentData(item.ItemEntity, out SimAssetId itemIDComponent))
                {
                    ItemVisualInfo itemInfo = ItemVisualInfoBank.Instance.GetItemInfoFromID(itemIDComponent);

                    GameAction.UseContext context = new GameAction.UseContext()
                    {
                        InstigatorPawn = SimWorldCache.LocalPawn,
                        InstigatorPawnController = SimWorldCache.LocalController,
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
                                                                          onItemPrimaryActionUsedCallback,
                                                                          onItemSecondaryActionUsedCallback,
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
        if (SimWorld.TryGetBufferReadOnly(SimWorldCache.LocalPawn, out DynamicBuffer<InventoryItemReference> inventory))
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
                        InstigatorPawn = SimWorldCache.LocalPawn,
                        InstigatorPawnController = SimWorldCache.LocalController,
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
        if (SimWorld.TryGetBufferReadOnly(SimWorldCache.LocalPawn, out DynamicBuffer<InventoryItemReference> inventory))
        {
            if (inventory.Length > ItemIndex && ItemIndex > -1)
            {
                int currentItemIndex = ItemIndex;
                ItemContextMenuDisplaySystem.Instance.ActivateContextMenuDisplay((int actionIndex) =>
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
