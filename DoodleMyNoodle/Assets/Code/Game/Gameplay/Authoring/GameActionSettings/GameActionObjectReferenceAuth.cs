using System;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

[RequiresEntityConversion]
[Serializable]
[GameActionSettingAuth(typeof(GameActionObjectReferenceSetting))]
public class GameActionObjectReferenceAuth : GameActionSettingAuthBase, IConvertGameObjectToEntity
{
    public GameObject ObjectReference;

    public override void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponentData(entity, new GameActionObjectReferenceSetting() { ObjectPrefab = conversionSystem.GetPrimaryEntity(ObjectReference) });
    }

    public override void DeclareReferencePrefabs(List<GameObject> referencedPrefabs) 
    {
        referencedPrefabs.Add(ObjectReference);
    }
}