using System;
using System.Collections.Generic;
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

public struct Settings_ViewBindingSystem_Binding : IBufferElementData
{
    public bool UseGameObjectInsteadOfEntity;
    public int PresentationGameObjectPrefabIndex;
    public Entity PresentationEntity;
    public SimAssetId SimAssetId;
}

public class Settings_ViewBindingSystem_BindingGameObjectList : 
    IComponentData, 
    IEquatable<Settings_ViewBindingSystem_BindingGameObjectList>
{
    public List<GameObject> PresentationGameObjects = new List<GameObject>();

    #region ECS Boilerplate
    public bool Equals(Settings_ViewBindingSystem_BindingGameObjectList other)
    {
        return ReferenceEquals(PresentationGameObjects, other.PresentationGameObjects);
    }

    public override int GetHashCode()
    {
        return -560777239 + EqualityComparer<List<GameObject>>.Default.GetHashCode(PresentationGameObjects);
    }
    #endregion
}