using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

[DisallowMultipleComponent]
[RequireComponent(typeof(SimAsset))]
public class LevelPlaylistAuth : MonoBehaviour, IConvertGameObjectToEntity
{
    public List<LevelDefinitionAuth> Levels = new List<LevelDefinitionAuth>();

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var levelEntities = dstManager.AddBuffer<LevelToAddToPlaylist>(entity);
        foreach (var level in Levels)
        {
            if (level != null)
                levelEntities.Add(new LevelToAddToPlaylist() { LevelId = level.GetComponent<SimAsset>().GetSimAssetId() });
        }
    }
}