using CCC.InspectorDisplay;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;


[DisallowMultipleComponent]
[RequiresEntityConversion]
public class ControllableAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject DefaultAIPrefab;

    [ShowIf(nameof(DefaultControllerPrefabIsValid), HideShowBaseAttribute.Type.Property)]
    public bool InstantiateAIOnSpawn = true;

    private bool DefaultControllerPrefabIsValid => DefaultAIPrefab != null;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<ControllableTag>(entity);

        if (DefaultAIPrefab)
        {
            dstManager.AddComponentData(entity, new DefaultControllerPrefab()
            {
                Value = conversionSystem.GetPrimaryEntity(DefaultAIPrefab)
            });

            if (InstantiateAIOnSpawn)
            {
                dstManager.AddComponent<InstantiateAndUseDefaultControllerTag>(entity);
            }
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        if (DefaultAIPrefab)
            referencedPrefabs.Add(DefaultAIPrefab);
    }
}
