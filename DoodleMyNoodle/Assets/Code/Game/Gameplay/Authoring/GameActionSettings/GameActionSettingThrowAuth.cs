using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using System;
using UnityEngine.Scripting.APIUpdating;

[Serializable]
[GameActionSettingAuth(typeof(GameActionSettingThrow))]
[MovedFrom(false, sourceClassName: "GameActionThrowSettingsAuth")]
public class GameActionSettingThrowAuth : GameActionSettingAuthBase
{
    public GameObject ProjectilePrefab;
    public fix SpeedMin = 0;
    public fix SpeedMax = 10;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionSettingThrow()
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