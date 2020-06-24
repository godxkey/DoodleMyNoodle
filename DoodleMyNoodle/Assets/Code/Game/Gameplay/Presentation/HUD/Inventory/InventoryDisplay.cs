
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class InventoryDisplay : GamePresentationBehaviour
{
    public Image Background;
    public Image BlockedDisplay;

    public List<InventorySlotInfo> InventorySlotShortcuts = new List<InventorySlotInfo>();

    public GameObject SlotsContainer;
    public GameObject InventorySlotPrefab;

    private List<InventorySlot> _inventorySlots = new List<InventorySlot>();

    public override void OnGameAwake()
    {
        base.OnGameAwake();

        for (int i = 0; i < InventorySlotShortcuts.Count; i++)
        {
            InventorySlot inventorySlot = Instantiate(InventorySlotPrefab, SlotsContainer.transform).GetComponent<InventorySlot>();
            _inventorySlots.Add(inventorySlot);
        }
    }

    public override void OnGameLateUpdate()
    {
        if(SimWorldCache.LocalPawn != Entity.Null && SimWorldCache.LocalController != Entity.Null)
        {
            UpdateInventorySlots();
        }
    }

    private void UpdateInventorySlots()
    {
        Action<int> onItemUsedCallback = null;

        if (!CommonReads.CanTeamPlay(SimWorld, SimWorld.GetComponentData<Team>(SimWorldCache.LocalController)))
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
        if (GameMonoBehaviourHelpers.GetSimulationWorld().TryGetBufferReadOnly(GamePresentationCache.Instance.LocalPawn, out DynamicBuffer<InventoryItemReference> inventory))
        {
            if(inventory.Length > ItemIndex && ItemIndex > -1)
            {
                InventoryItemReference item = inventory[ItemIndex];
                if (GameMonoBehaviourHelpers.GetSimulationWorld().TryGetComponentData(item.ItemEntity, out SimAssetId itemIDComponent))
                {
                    Entity itemEntity = item.ItemEntity;

                    SimWorld.TryGetComponentData(itemEntity, out GameActionId actionId);
                    GameAction itemGameAction = GameActionBank.GetAction(actionId);

                    GameAction.UseContext useContext = new GameAction.UseContext()
                    {
                        InstigatorPawn = SimWorldCache.LocalPawn,
                        InstigatorPawnController = SimWorldCache.LocalController,
                        ItemEntity = itemEntity
                    };

                    GameAction.UseContract itemUseContract = itemGameAction.GetUseContract(SimWorld, useContext);

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

        IdentifyAndGatherDataForParameterDescription(itemUseContact.ParameterTypes[DataToExtract], DataToExtract, ()=>
        {
            if (DataToExtract >= itemUseContact.ParameterTypes.Length)
            {
                onComplete?.Invoke();
                return;
            }

            // Little Delay between choices
            this.DelayedCall(0.1f, ()=> { QueryUseDataFromPlayer(itemUseContact, onComplete, (byte)(DataToExtract + 1)); });
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
