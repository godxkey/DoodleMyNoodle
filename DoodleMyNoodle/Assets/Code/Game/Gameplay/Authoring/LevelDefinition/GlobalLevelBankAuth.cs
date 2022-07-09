using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class GlobalLevelBankAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GlobalLevelBank Bank;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GlobalLevelBankTag());
        var prefabBuffer = dstManager.AddBuffer<GlobalLevelBankEntry>(entity);

        foreach (LevelDefinitionAuth item in Bank.Levels)
        {
            if (item == null)
                continue;

            Entity itemPrefabEntity = conversionSystem.GetPrimaryEntity(item.gameObject);

            if (itemPrefabEntity == Entity.Null)
                continue;

            prefabBuffer.Add(new GlobalLevelBankEntry()
            {
                LevelDefinitionPrefab = itemPrefabEntity
            });
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        if (Bank != null)
        {
            foreach (LevelDefinitionAuth item in Bank.Levels)
            {
                if (item != null)
                {
                    referencedPrefabs.Add(item.gameObject);
                }
            }
        }
    }
}