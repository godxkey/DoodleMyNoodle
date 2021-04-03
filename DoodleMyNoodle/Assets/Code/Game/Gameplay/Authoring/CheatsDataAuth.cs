using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class CheatsDataAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [SerializeField] private ItemBank _itemBank; // TODO: automate this

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var allItemsBuffer = dstManager.AddBuffer<CheatsAllItemElement>(entity);

        _itemBank.Validate();

        foreach (var item in _itemBank.Items)
        {
            if (item.gameObject == null)
                continue;

            allItemsBuffer.Add(conversionSystem.GetPrimaryEntity(item));
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        foreach (var item in _itemBank.Items)
        {
            if(item.gameObject == null)
                continue;

            referencedPrefabs.Add(item.gameObject);
        }
    }
}