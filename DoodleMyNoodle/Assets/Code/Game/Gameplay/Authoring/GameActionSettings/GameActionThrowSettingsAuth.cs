using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using System;

[Serializable]
[GameActionSettingAuth(typeof(GameActionThrowSettings))]
public class GameActionThrowSettingsAuth : GameActionSettingAuthBase
{
    public GameObject ProjectilePrefab;
    public fix SpeedMin = 0;
    public fix SpeedMax = 10;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionThrowSettings()
        {
            SpeedMax = SpeedMax,
            SpeedMin = SpeedMin,
            ProjectilePrefab = conversionSystem.GetPrimaryEntity(ProjectilePrefab),
        });
    }

    public override void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(ProjectilePrefab);
    }
}