using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;

[DisallowMultipleComponent]
[RequiresEntityConversion]
public class GameActionThrowSettingsAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public GameObject ProjectilePrefab;
    public fix SpeedMin = 0;
    public fix SpeedMax = 10;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionThrowSettings()
        {
            SpeedMax = SpeedMax,
            SpeedMin = SpeedMin,
            ProjectilePrefab = conversionSystem.GetPrimaryEntity(ProjectilePrefab),
        });
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(ProjectilePrefab);
    }
}