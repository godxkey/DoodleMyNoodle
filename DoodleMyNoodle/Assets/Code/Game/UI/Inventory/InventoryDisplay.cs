using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class InventoryDisplay : GameMonoBehaviour
{
    public List<InventorySlotInfo> InventorySlotShortcuts = new List<InventorySlotInfo>();

    public GameObject SlotsContainer;
    public GameObject InventorySlotPrefab;

    private List<InventorySlot> _inventorySlots = new List<InventorySlot>();

    private Entity _localPawn = Entity.Null;

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
        UpdateCurrentPlayerPawn();

        int itemIndex = 0;
        if (GameMonoBehaviourHelpers.SimulationWorld.TryGetBuffer(_localPawn, out DynamicBuffer<InventoryItemReference> inventory))
        {
            foreach (InventoryItemReference item in inventory)
            {
                if (GameMonoBehaviourHelpers.SimulationWorld.TryGetComponentData(item.ItemEntity,out SimAssetId itemIDComponent))
                {
                    ItemVisualInfo itemInfo = ItemVisualInfoBank.Instance.GetItemInfoFromID(itemIDComponent);
                    _inventorySlots[itemIndex].UpdateCurrentItemSlot(itemInfo, InventorySlotShortcuts[itemIndex], OnItemUsed);
                    itemIndex++;
                }
            }
        }

        // Clear the rest of the inventory slots
        for (int i = _inventorySlots.Count - 1; i > itemIndex; i--)
        {
            _inventorySlots[i].UpdateCurrentItemSlot(null, InventorySlotShortcuts[i], null);
        }
    }

    private void UpdateCurrentPlayerPawn()
    {
        if(_localPawn == Entity.Null)
        {
            _localPawn = PlayerIdHelpers.GetLocalSimPawnEntity(GameMonoBehaviourHelpers.SimulationWorld);
        }
    }

    private void OnItemUsed(SimAssetId ItemAssetID)
    {
        if (GameMonoBehaviourHelpers.SimulationWorld.TryGetBuffer(_localPawn, out DynamicBuffer<InventoryItemReference> inventory))
        {
            foreach (InventoryItemReference item in inventory)
            {
                if (GameMonoBehaviourHelpers.SimulationWorld.TryGetComponentData(item.ItemEntity, out SimAssetId itemIDComponent))
                {
                    if (itemIDComponent == ItemAssetID)
                    {
                        OnStartUsingNewItem();

                        Entity itemEntity = item.ItemEntity;

                        GameMonoBehaviourHelpers.SimulationWorld.TryGetComponentData(itemEntity, out GameActionId actionId);
                        GameAction itemGameAction = GameActionBank.GetAction(actionId);

                        GameAction.UseContract itemUseContract = itemGameAction.GetUseContract(GameMonoBehaviourHelpers.SimulationWorld, _localPawn);

                        QueryUseDataFromPlayer(itemUseContract);
                        GameAction.UseData itemUseData = _currentItemUseData;

                        // USE ITEM - SIM INPUT
                        //itemGameAction.Use(GameMonoBehaviourHelpers.SimulationWorld, _localPawn, itemUseData);
                    }
                }
            }
        }
    }

    private void OnStartUsingNewItem()
    {
        _currentItemUseData = new GameAction.UseData() { ParameterDatas = new GameAction.ParameterData[] { } };
    }

    private GameAction.UseData _currentItemUseData;
    private void QueryUseDataFromPlayer(GameAction.UseContract itemUseContact, int DataToExtract = 0)
    {
        if (itemUseContact.ParameterTypes.Length >= DataToExtract)
            return;

        IdentifyAndGatherDataForParameterDescription(itemUseContact.ParameterTypes[DataToExtract], DataToExtract, ()=> 
        {
            QueryUseDataFromPlayer(itemUseContact, DataToExtract + 1);
        });
    }
    
    private void IdentifyAndGatherDataForParameterDescription(GameAction.ParameterDescription parameterDescription, int index, Action OnComplete)
    {
        GameActionParameterTile.Description TileDescription = (GameActionParameterTile.Description)parameterDescription;
        if (TileDescription != null)
        {
            TileHighlightManager.Instance.AskForSingleTileSelectionAroundPlayer(TileDescription,(GameActionParameterTile.Data TileSelectedData) => 
            {
                _currentItemUseData.ParameterDatas[index] = TileSelectedData;
                OnComplete?.Invoke();
            });
        }

        // other types of Parameter Description here ...
    }
}
