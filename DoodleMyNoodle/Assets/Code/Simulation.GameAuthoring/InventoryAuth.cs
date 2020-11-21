using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngineX;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class InventoryAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [FormerlySerializedAs("Size")]
    public int Capacity = 6;

    public List<GameObject> InitialItems = new List<GameObject>();

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new InventoryCapacity() { Value = Capacity });

        dstManager.AddBuffer<InventoryItemReference>(entity);

        DynamicBuffer<StartingInventoryItem> startingInventory = dstManager.AddBuffer<StartingInventoryItem>(entity);

        foreach (GameObject itemGO in InitialItems)
        {
            if (startingInventory.Length >= Capacity)
                break;

            if (!itemGO)
                continue;

            startingInventory.Add(new StartingInventoryItem() { ItemEntityPrefab = conversionSystem.GetPrimaryEntity(itemGO) });
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.AddRange(InitialItems);
    }
}