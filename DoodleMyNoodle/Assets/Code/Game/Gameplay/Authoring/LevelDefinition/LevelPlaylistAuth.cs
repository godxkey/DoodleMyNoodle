using System.Collections.Generic;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;

[DisallowMultipleComponent]
[RequireComponent(typeof(SimAsset))]
public class LevelPlaylistAuth : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public List<LevelDefinition> Levels = new List<LevelDefinition>();

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var levelEntities = dstManager.AddBuffer<LevelToAddToPlaylist>(entity);
        foreach (var level in Levels)
        {
            if (level != null)
                levelEntities.Add(new LevelToAddToPlaylist() { LevelDefinition = conversionSystem.GetPrimaryEntity(level) });
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        foreach (var level in Levels)
        {
            if (level != null)
                referencedPrefabs.Add(level.gameObject);
        }
    }
}