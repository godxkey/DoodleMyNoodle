using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class InteractableInventoryPrefabReferenceAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject InventoryPrefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        Entity inventoryAddonEntity = conversionSystem.GetPrimaryEntity(InventoryPrefab);
        InteractableInventoryPrefabReferenceSingletonComponent inventoryAddonReference = new InteractableInventoryPrefabReferenceSingletonComponent() { Prefab = inventoryAddonEntity };
        dstManager.AddComponentData(entity, inventoryAddonReference);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(InventoryPrefab);
    }
}
