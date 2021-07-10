using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngine.Serialization;
using UnityEngineX;

[Serializable]
[GameActionSettingAuth(typeof(GameActionSettingEntityReference))]
public class GameActionSettingEntityReferenceAuth : GameActionSettingAuthBase
{
    public GameObject Entity;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (Entity == null)
        {
            Log.Error($"Invalid reference in {nameof(GameActionSettingEntityReferenceAuth)}", Context);
        }

        dstManager.AddComponentData(entity, new GameActionSettingEntityReference() { EntityPrefab = conversionSystem.GetPrimaryEntity(Entity) });
    }

    public override void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(Entity);
    }
}