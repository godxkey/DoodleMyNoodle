using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Unity.Entities;

public class GameActionSettingAuthAttribute : Attribute
{
    public Type SettingType;

    public GameActionSettingAuthAttribute(Type settingType) { SettingType = settingType; }
}

[Serializable]
public abstract class GameActionSettingAuthBase
{
    // mandatory
    public abstract void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem);

    // optional
    public virtual void DeclareReferencePrefabs(List<GameObject> referencedPrefabs) { }
}