using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Scripting.APIUpdating;
using UnityEngineX;

[Serializable]
[GameActionSettingAuth(typeof(GameActionSettingObjectReference))]
[MovedFrom(false, sourceClassName: "GameActionObjectReferenceAuth")]
public class GameActionSettingObjectReferenceAuth : GameActionSettingAuthBase
{
    public GameObject ObjectReference;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (ObjectReference == null)
        {
            Log.Error($"Invalid reference in {nameof(GameActionSettingObjectReferenceAuth)}");
        }

        dstManager.AddComponentData(entity, new GameActionSettingObjectReference() { ObjectPrefab = conversionSystem.GetPrimaryEntity(ObjectReference) });
    }

    public override void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(ObjectReference);
    }
}