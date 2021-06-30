using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[DisallowMultipleComponent]
public class PlayerPawnPrefabReferenceAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject PlayerPrefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        Entity playerPawnEntity = conversionSystem.GetPrimaryEntity(PlayerPrefab);
        PlayerPawnPrefabReferenceSingletonComponent playerPawnReference = new PlayerPawnPrefabReferenceSingletonComponent() { Prefab = playerPawnEntity };
        dstManager.AddComponentData(entity, playerPawnReference);
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(PlayerPrefab);
    }
}
