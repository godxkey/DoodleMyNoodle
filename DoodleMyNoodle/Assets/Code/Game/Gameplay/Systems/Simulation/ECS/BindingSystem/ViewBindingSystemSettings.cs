using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

//[assembly: RegisterGenericComponentType(typeof(BlobAssetReferenceComponent<ViewBindingSystemSettings>))]

//[Serializable]
//public struct ViewBindingSystemSettings
//{
//    public BlobArray<Entity> BlueprintPresentationEntities;
//    public BlobArray<int> BlueprintIds;
//}

public struct Settings_ViewBindingSystem_BlueprintDefinition : IBufferElementData
{
    public Entity PresentationEntity;
    public int BlueprintId;
}