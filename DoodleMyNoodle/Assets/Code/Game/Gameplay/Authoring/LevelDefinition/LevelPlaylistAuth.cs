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
    public GlobalGameplaySettings GlobalGameplaySettings;

    private List<LevelData> _levelDatas;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        using var _ = ListPool<Entity>.Take(out var createdLevels);

        foreach (var levelData in GetOrCreateLevelDatas())
        {
            var levelEntity = dstManager.CreateEntity(typeof(LevelDefinitionMobSpawn));
#if UNITY_EDITOR
            dstManager.SetName(levelEntity, levelData.DebugName);
#endif
            var spawnsBuffer = dstManager.GetBuffer<LevelDefinitionMobSpawn>(levelEntity);

            foreach (var spawn in levelData.MobSpawns)
            {
                spawnsBuffer.Add(new LevelDefinitionMobSpawn()
                {
                    Flags = spawn.MobModifierFlags,
                    MobToSpawn = conversionSystem.GetPrimaryEntity(spawn.SimAsset.gameObject),
                    Position = (fix2)(Vector2)spawn.Position,
                });
            }

            createdLevels.Add(levelEntity);
        }

        var levelsEntity = dstManager.AddBuffer<LevelToAddToPlaylist>(entity);
        foreach (var levelEntity in createdLevels)
        {
            levelsEntity.Add(new LevelToAddToPlaylist() { LevelDefinition = levelEntity });
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        HashSet<GameObject> mobPrefabs = new HashSet<GameObject>();
        foreach (var levelData in GetOrCreateLevelDatas())
        {
            foreach (var spawn in levelData.MobSpawns)
            {
                mobPrefabs.Add(spawn.SimAsset.gameObject);
            }
        }

        referencedPrefabs.AddRange(mobPrefabs);
    }

    private List<LevelData> GetOrCreateLevelDatas()
    {
        if (_levelDatas == null)
        {
            _levelDatas = new List<LevelData>(Levels.Count);
            foreach (var level in Levels)
            {
                _levelDatas.Add(level.GenerateLevelData(GlobalGameplaySettings));
            }
        }

        return _levelDatas;
    }
}