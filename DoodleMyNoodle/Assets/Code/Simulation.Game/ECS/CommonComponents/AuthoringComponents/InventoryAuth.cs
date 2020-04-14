using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class InventoryAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public List<GameObject> InitialItems = new List<GameObject>();

    private void Awake()
    {
        //for (int i = 0; i < InitialItems.Count; i++)
        //{
        //    InitialItems[i] = GameObject.Instantiate(InitialItems[i]);
        //}
    }

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        //List<Entity> itemEntities = new List<Entity>(InitialItems.Count);

        //foreach (GameObject itemGO in InitialItems)
        //{
        //    itemEntities.Add(dstManager.Instantiate(conversionSystem.GetPrimaryEntity(itemGO)));
        //}

        DynamicBuffer<StartingInventoryItem> inventory = dstManager.AddBuffer<StartingInventoryItem>(entity);

        foreach (GameObject itemGO in InitialItems)
        //foreach (Entity itemEntity in itemEntities)
        {
            inventory.Add(new StartingInventoryItem() { ItemEntityPrefab = conversionSystem.GetPrimaryEntity(itemGO) });

            //inventory.Add(new InventoryItemReference()
            //{
            //    ItemEntity = itemEntity
            //});
        }
        //foreach (GameObject itemGO in InitialItems)
        //{
        //    Entity itemEntity = conversionSystem.GetPrimaryEntity(itemGO);
        //    conversionSystem.CreateAdditionalEntity(itemGO);
        //    conversionSystem.CreateAdditionalEntity(itemGO);
        //    conversionSystem.CreateAdditionalEntity(itemGO);

        //    inventory.Add(new InventoryItemReference()
        //    {
        //        ItemEntity = itemEntity
        //    });
        //}
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.AddRange(InitialItems);
    }
}