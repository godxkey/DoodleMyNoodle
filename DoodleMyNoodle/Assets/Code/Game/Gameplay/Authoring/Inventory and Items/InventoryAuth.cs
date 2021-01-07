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
    public enum FillType
    {
        GlobalItemBank,
        CustomItemBank,
        CustomList
    }

    [FormerlySerializedAs("Size")]
    public int Capacity = 6;

    public FillType FillingType = FillType.CustomList;

    private bool IsGlobalItemBank { get { return FillingType == FillType.GlobalItemBank; } }
    private bool IsCustomItemBank { get { return FillingType == FillType.CustomItemBank; } }
    private bool IsRandom { get { return (IsGlobalItemBank || IsCustomItemBank); } }

    [ShowIf("IsRandom")]
    public int ConsumablesMaxAmount = 3;

    [ShowIf("IsCustomItemBank")]
    public ItemBank CustomItemBank;

    [HideIf("IsRandom")] // TODO: Not working with list ?
    public List<GameObject> InitialItems = new List<GameObject>();

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new InventoryCapacity() { Value = Capacity });

        dstManager.AddBuffer<InventoryItemReference>(entity);

        switch (FillingType)
        {
            case FillType.GlobalItemBank:

                dstManager.AddComponentData(entity, new RandomInventoryInfo() { MaxConsumableAmount = ConsumablesMaxAmount });

                break;

            case FillType.CustomItemBank:

                dstManager.AddComponentData(entity, new RandomInventoryInfo() { MaxConsumableAmount = ConsumablesMaxAmount });

                DynamicBuffer<ItemBankPrefabReference> itemBank = dstManager.AddBuffer<ItemBankPrefabReference>(entity);

                foreach (GameObject item in CustomItemBank.Items)
                {
                    Entity itemEntity = conversionSystem.GetPrimaryEntity(item);

                    ItemBankPrefabReference newItemPrefabReference = new ItemBankPrefabReference() { ItemEntityPrefab = itemEntity };

                    itemBank.Add(newItemPrefabReference);
                }

                break;

            case FillType.CustomList:

                DynamicBuffer<StartingInventoryItem> startingInventory = dstManager.AddBuffer<StartingInventoryItem>(entity);

                foreach (GameObject itemGO in InitialItems)
                {
                    if (startingInventory.Length >= Capacity)
                        break;

                    if (!itemGO)
                        continue;

                    startingInventory.Add(new StartingInventoryItem() { ItemEntityPrefab = conversionSystem.GetPrimaryEntity(itemGO) });
                }

                break;

            default:

                break;
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        switch (FillingType)
        {
            case FillType.CustomItemBank:

                if (CustomItemBank != null)
                {
                    referencedPrefabs.AddRange(CustomItemBank.Items);
                }

                break;
            case FillType.CustomList:
                referencedPrefabs.AddRange(InitialItems);
                break;
            default:
                break;
        }
    }
}