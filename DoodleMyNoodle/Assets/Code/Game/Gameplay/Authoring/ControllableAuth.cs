using CCC.InspectorDisplay;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


[DisallowMultipleComponent]
public class ControllableAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject DefaultAIPrefab;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        Entity defaultController = Entity.Null;

        if (DefaultAIPrefab != null)
        {
            defaultController = conversionSystem.GetPrimaryEntity(DefaultAIPrefab);
        }

        dstManager.AddComponent<Controllable>(entity);
        dstManager.AddComponentData(entity, new DefaultControllerPrefab() { Value = defaultController });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        if (DefaultAIPrefab)
            referencedPrefabs.Add(DefaultAIPrefab);
    }
}
