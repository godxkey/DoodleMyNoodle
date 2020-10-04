using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class CheatsDataAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    [SerializeField] private List<GameActionIdAuth> _allItems; // TODO: automate this

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var allItemsBuffer = dstManager.AddBuffer<CheatsAllItemElement>(entity);

        foreach (var item in _allItems)
        {
            allItemsBuffer.Add(conversionSystem.GetPrimaryEntity(item));
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        foreach (var item in _allItems)
        {
            referencedPrefabs.Add(item.gameObject);
        }
    }
}

public struct CheatsAllItemElement : IBufferElementData
{
    public Entity ItemPrefab;

    public static implicit operator Entity(CheatsAllItemElement val) => val.ItemPrefab;
    public static implicit operator CheatsAllItemElement(Entity val) => new CheatsAllItemElement() { ItemPrefab = val };
}