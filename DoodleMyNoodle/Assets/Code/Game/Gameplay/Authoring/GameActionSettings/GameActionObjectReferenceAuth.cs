using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngineX;

[RequiresEntityConversion]
[Serializable]
[GameActionSettingAuth(typeof(GameActionObjectReferenceSetting))]
public class GameActionObjectReferenceAuth : GameActionSettingAuthBase
{
    public GameObject ObjectReference;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        if (ObjectReference == null)
        {
            Log.Error($"Invalid reference in {nameof(GameActionObjectReferenceAuth)}");
        }

        dstManager.AddComponentData(entity, new GameActionObjectReferenceSetting() { ObjectPrefab = conversionSystem.GetPrimaryEntity(ObjectReference) });
    }

    public override void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        referencedPrefabs.Add(ObjectReference);
    }
}