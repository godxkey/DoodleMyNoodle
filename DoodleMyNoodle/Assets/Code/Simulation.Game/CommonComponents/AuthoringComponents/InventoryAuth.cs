using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class InventoryAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public List<GameObject> InitialItems = new List<GameObject>();

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        DynamicBuffer<StartingInventoryItem> inventory = dstManager.AddBuffer<StartingInventoryItem>(entity);

        foreach (GameObject itemGO in InitialItems)
        {
            inventory.Add(new StartingInventoryItem() { ItemEntityPrefab = conversionSystem.GetPrimaryEntity(itemGO) });
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.AddRange(InitialItems);
    }
}