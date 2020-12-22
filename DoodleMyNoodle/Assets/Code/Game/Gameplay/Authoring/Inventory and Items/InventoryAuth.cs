using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngineX;
using CCC.InspectorDisplay;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class InventoryAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [FormerlySerializedAs("Size")]
    public int Capacity = 6;

    public bool FillWithRandomItems = false;

    [ShowIf("FillWithRandomItems")]
    public int AmountToFill = 2;

    [ShowIf("FillWithRandomItems")]
    public int ConsumablesMaxAmount = 3;

    [ShowIf("FillWithRandomItems")]
    public ItemBank Bank;

    [HideIf("FillWithRandomItems")]
    public List<GameObject> InitialItems = new List<GameObject>();

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new InventoryCapacity() { Value = Capacity });

        dstManager.AddBuffer<InventoryItemReference>(entity);

        DynamicBuffer<StartingInventoryItem> startingInventory = dstManager.AddBuffer<StartingInventoryItem>(entity);

        if (FillWithRandomItems)
        {
            List<GameObject> selectedItems = Bank.GetRandomItems(AmountToFill, ConsumablesMaxAmount);
            foreach (GameObject itemGO in selectedItems)
            {
                if (startingInventory.Length >= Capacity)
                    break;

                if (!itemGO)
                    continue;

                startingInventory.Add(new StartingInventoryItem() { ItemEntityPrefab = conversionSystem.GetPrimaryEntity(itemGO) });
            }
        }
        else
        {
            foreach (GameObject itemGO in InitialItems)
            {
                if (startingInventory.Length >= Capacity)
                    break;

                if (!itemGO)
                    continue;

                startingInventory.Add(new StartingInventoryItem() { ItemEntityPrefab = conversionSystem.GetPrimaryEntity(itemGO) });
            }
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.AddRange(InitialItems);
        if (Bank != null)
        {
            referencedPrefabs.AddRange(Bank.Items);
        }
    }
}