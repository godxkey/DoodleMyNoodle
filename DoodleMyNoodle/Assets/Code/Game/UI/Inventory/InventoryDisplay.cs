﻿
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : GameMonoBehaviour
{
    public Image Background;
    public Image BlockedDisplay;

    public List<InventorySlotInfo> InventorySlotShortcuts = new List<InventorySlotInfo>();

    public GameObject SlotsContainer;
    public GameObject InventorySlotPrefab;

    private List<InventorySlot> _inventorySlots = new List<InventorySlot>();

    public override void OnGameReady()
    {
        base.OnGameReady();

        for (int i = 0; i < InventorySlotShortcuts.Count; i++)
        {
            InventorySlot inventorySlot = Instantiate(InventorySlotPrefab, SlotsContainer.transform).GetComponent<InventorySlot>();
            _inventorySlots.Add(inventorySlot);
        }
    }

    public override void OnGameUpdate()
    {
        if(GamePresentationCache.Instance.LocalPawn != Entity.Null)
        {
            UpdateInventorySlots();
        }
    }

    private void UpdateInventorySlots()
    {
        Action<int> onItemUsedCallback = null;

        Entity pawnController = CommonReads.GetPawnController(SimWorld, GamePresentationCache.Instance.LocalPawn);
        if (!CommonReads.CanTeamPlay(SimWorld, SimWorld.GetComponentData<Team>(pawnController)))
        {
            TileHighlightManager.Instance.InterruptTileSelectionProcess();
            Background.color = Color.white;
            BlockedDisplay.gameObject.SetActive(true);
        }
        else
        {
            onItemUsedCallback = OnIntentionToUseItem;
            Background.color = Color.green;
            BlockedDisplay.gameObject.SetActive(false);

            VerifyButtonInputForSlots();
        }

        int itemIndex = 0;
        if (SimWorld.TryGetBufferReadOnly(GamePresentationCache.Instance.LocalPawn, out DynamicBuffer<InventoryItemReference> inventory))
        {
            foreach (InventoryItemReference item in inventory)
            {
                if (SimWorld.TryGetComponentData(item.ItemEntity, out SimAssetId itemIDComponent))
                {
                    ItemVisualInfo itemInfo = ItemVisualInfoBank.Instance.GetItemInfoFromID(itemIDComponent);

                    _inventorySlots[itemIndex].UpdateCurrentItemSlot(itemInfo, itemIndex, InventorySlotShortcuts[itemIndex], onItemUsedCallback);
                    itemIndex++;
                }
            }
        }

        // Clear the rest of the inventory slots
        for (int i = _inventorySlots.Count - 1; i >= itemIndex; i--)
        {
            _inventorySlots[i].UpdateCurrentItemSlot(null, i, InventorySlotShortcuts[i], null);
        }
    }

    private void VerifyButtonInputForSlots()
    {
        // We permit one input per frame
        for (int i = 0; i < InventorySlotShortcuts.Count; i++)
        {
            if (Input.GetKeyDown(InventorySlotShortcuts[i].InputShortcut))
            {
                if(_inventorySlots.Count > i)
                {
                    _inventorySlots[i].UseInventorySlot();
                }
                return;
            }
        }
    }

    private void OnIntentionToUseItem(int ItemIndex)
    {
        if (GameMonoBehaviourHelpers.SimulationWorld.TryGetBufferReadOnly(GamePresentationCache.Instance.LocalPawn, out DynamicBuffer<InventoryItemReference> inventory))
        {
            if(inventory.Length > ItemIndex && ItemIndex > -1)
            {
                InventoryItemReference item = inventory[ItemIndex];
                if (GameMonoBehaviourHelpers.SimulationWorld.TryGetComponentData(item.ItemEntity, out SimAssetId itemIDComponent))
                {
                    Entity itemEntity = item.ItemEntity;

                    SimWorld.TryGetComponentData(itemEntity, out GameActionId actionId);
                    GameAction itemGameAction = GameActionBank.GetAction(actionId);

                    GameAction.UseContract itemUseContract = itemGameAction.GetUseContract(SimWorld, PlayerHelpers.GetLocalSimPlayerEntity(SimWorld), GamePresentationCache.Instance.LocalPawn);

                    OnStartUsingNewItem(itemUseContract);

                    QueryUseDataFromPlayer(itemUseContract,()=> 
                    {
                        SimPlayerInputUseItem simInput = new SimPlayerInputUseItem(ItemIndex, _currentItemUseData);
                        SimWorld.SubmitInput(simInput);
                    });

                }
            }
        }
    }

    private void OnStartUsingNewItem(GameAction.UseContract NewItemContact)
    {
        // clean up in case we try to use two items one after the other (cancel feature)
        _currentItemUseData = GameAction.UseData.Create(new GameActionParameterTile.Data[NewItemContact.ParameterTypes.Length]);
    }

    private GameAction.UseData _currentItemUseData;
    private void QueryUseDataFromPlayer(GameAction.UseContract itemUseContact, Action onComplete, int DataToExtract = 0)
    {
        if (DataToExtract >= itemUseContact.ParameterTypes.Length) 
        {
            onComplete?.Invoke();
            return;
        }

        IdentifyAndGatherDataForParameterDescription(itemUseContact.ParameterTypes[DataToExtract], DataToExtract, ()=> 
        {
            QueryUseDataFromPlayer(itemUseContact, onComplete, DataToExtract + 1);
        });
    }
    
    private void IdentifyAndGatherDataForParameterDescription(GameAction.ParameterDescription parameterDescription, int index, Action OnComplete)
    {
        // SELECT A SINGLE TILE
        if (parameterDescription is GameActionParameterTile.Description TileDescription)
        {
            if (TileDescription != null)
            {
                TileHighlightManager.Instance.AskForSingleTileSelectionAroundPlayer(TileDescription, (GameActionParameterTile.Data TileSelectedData) =>
                {
                    _currentItemUseData.ParameterDatas[index] = TileSelectedData;
                    OnComplete?.Invoke();
                });
                return;
            }
        }

        // SELF TARGETING
        if (parameterDescription is GameActionParameterSelfTarget.Description SelfDescription)
        {
            _currentItemUseData.ParameterDatas[index] = new GameActionParameterSelfTarget.Popo(0);
            OnComplete?.Invoke();
            return;
        }

        // other types of Parameter Description here ...
    }
}
