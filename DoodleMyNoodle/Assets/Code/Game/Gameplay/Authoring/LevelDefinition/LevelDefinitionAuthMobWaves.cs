using CCC.InspectorDisplay;
using System;
using System.Collections.Generic;
using Unity.Assertions;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngineX;
using UnityEngineX.InspectorDisplay;

[DisallowMultipleComponent, RequireComponent(typeof(LevelDefinitionAuth))]
public class LevelDefinitionAuthMobWaves : MonoBehaviour, IConvertGameObjectToEntity, IDeclareReferencedPrefabs
{
    public enum GroupDensity
    {
        Single,
        Low,
        Normal,
        High
    }

    public enum WaveSize
    {
        Small,
        Normal,
        Large
    }

    public enum MobPrefab
    {
        Archer,
        Bird,
        Brute,
        BruteBoss,
        Chicken,
        Frog,
    }

    [System.Serializable]
    public class MobArchetype
    {
        public MobPrefab Type = MobPrefab.Brute;
        public MobSpawmModifierFlags Modifiers = MobSpawmModifierFlags.None;
    }

    [System.Serializable]
    public class MobGroup
    {
        public Vector2 Range = new Vector2(0, 0.5f);
        public GroupDensity Density = GroupDensity.Normal;
        [AlwaysExpand]
        public MobArchetype Mob = new MobArchetype();
    }

    [System.Serializable]
    public class MobWave
    {
        public WaveSize WaveSize = WaveSize.Normal;
        public List<MobGroup> Groups = new List<MobGroup>() { new MobGroup() };
    }

    public float TotalEnemies = 30f;
    public float WaveSpacing = 10f;
    //public MobArchetype WaveSpacingMob = new MobArchetype();
    public List<MobWave> Waves = new List<MobWave>() { new MobWave() };

    [System.NonSerialized]
    public string LastGenerationError;

    private float ToWeight(WaveSize size)
        => size switch
        {
            WaveSize.Small => 1,
            WaveSize.Normal => 2,
            WaveSize.Large => 3,
            _ => 0,
        };

    private float MobPerMeter(GroupDensity density)
        => density switch
        {
            GroupDensity.Single => 0f,
            GroupDensity.Low => 0.4f,
            GroupDensity.Normal => 0.7f,
            GroupDensity.High => 1,
            _ => 0,
        };

    public LevelMobWaveData GenerateLevelData(GlobalGameplaySettings globalSettings)
    {
        LastGenerationError = null;
        LevelMobWaveData result = new LevelMobWaveData();

        float availableMobCount = TotalEnemies;

        float[] wavesLengthWeights = new float[Waves.Count];
        float[] waveLengths = new float[Waves.Count];
        float[][] waveGroupMobCountWeights = new float[Waves.Count][];
        float[][] waveGroupMobCountsActual = new float[Waves.Count][];

        for (int w = 0; w < Waves.Count; w++)
        {
            var wave = Waves[w];
            waveGroupMobCountsActual[w] = new float[wave.Groups.Count];
            waveGroupMobCountWeights[w] = new float[wave.Groups.Count];
        }

        // give 1 mob to each group (which is the minimum)
        for (int w = 0; w < Waves.Count; w++)
        {
            var wave = Waves[w];
            for (int g = 0; g < wave.Groups.Count; g++)
            {
                waveGroupMobCountsActual[w][g] = 1;
                availableMobCount--;
            }
        }

        // distribute other mobs according to weight
        float waveLengthPerWeight = 0;

        // if the available amount of mob is under 0.49f, increase it. This will make it possible to calculate the sizes of the waves. Otherwise, we divide by 0.
        // 0.49f is just low enough to make sure RoundToInt(..) doesn't add an extra unwanted mob.
        availableMobCount = Mathf.Max(availableMobCount, 0.49f);

        // Determine the weight of each mob group in terms of mob count (higher weight means they will get more mobs out of the available ones)
        for (int w = 0; w < Waves.Count; w++)
        {
            var wave = Waves[w];
            wavesLengthWeights[w] = ToWeight(wave.WaveSize);

            for (int g = 0; g < wave.Groups.Count; g++)
            {
                var group = wave.Groups[g];
                float r = group.Range.y - group.Range.x;

                waveGroupMobCountWeights[w][g] = r * MobPerMeter(group.Density) * wavesLengthWeights[w];
            }
        }

        // Count total weight of all wave groups
        float totalWeight = 0;
        for (int w = 0; w < Waves.Count; w++)
        {
            totalWeight += waveGroupMobCountWeights[w].Sum();
        }

        if (totalWeight > 0)
        {
            // Calculate the amount of mobs each group should be given (in addition to the startin '1' mob).
            // Also, from this value, determine the length of the waves
            for (int w = 0; w < Waves.Count; w++)
            {
                var wave = Waves[w];
                for (int g = 0; g < wave.Groups.Count; g++)
                {
                    // mobs to add
                    float mobsGrantedByGroupSize = availableMobCount * waveGroupMobCountWeights[w][g] / totalWeight;

                    // add mobs
                    waveGroupMobCountsActual[w][g] += mobsGrantedByGroupSize;

                    // from the amount calculated, determine the size of the group
                    float groupLength = mobsGrantedByGroupSize / MobPerMeter(Waves[w].Groups[g].Density);

                    // If the group size is not 0, we can use this value to determine the size of the wave
                    if (groupLength > 0.01 && waveLengths[w] == 0)
                    {
                        float waveGroupRatio = wave.Groups[g].Range.y - wave.Groups[g].Range.x;
                        waveLengthPerWeight = (groupLength / waveGroupRatio) / wavesLengthWeights[w];
                    }
                }
            }
        }
        else
        {
            // Since all wave groups have 1 mob in them, it's impossible to calculate the wave lengths using the density of the groups.
            // Hard code at 5 by default
            waveLengthPerWeight = 5f;
        }

        // calculate wave lengths
        for (int w = 0; w < Waves.Count; w++)
        {
            waveLengths[w] = wavesLengthWeights[w] * waveLengthPerWeight;
        }

        float waveStartingPos = (float)SimulationGameConstants.EnemySpawnDistanceFromPlayerGroup;
        for (int w = 0; w < Waves.Count; w++)
        {
            MobWave wave = Waves[w];

            for (int g = 0; g < wave.Groups.Count; g++)
            {
                int mobCount = Mathf.RoundToInt(waveGroupMobCountsActual[w][g]);

                Assert.AreNotEqual(0, mobCount);

                float startPosition = waveStartingPos + (waveLengths[w] * wave.Groups[g].Range.x);
                float endPosition = waveStartingPos + (waveLengths[w] * wave.Groups[g].Range.y);

                EmplaceGroup(wave.Groups[g], mobCount, startPosition, endPosition, result, globalSettings);
            }

            waveStartingPos += waveLengths[w] + WaveSpacing;
        }

        return result;
    }

    private void EmplaceGroup(MobGroup group, int mobCount, float min, float max, LevelMobWaveData levelData, GlobalGameplaySettings globalSettings)
    {
        float position = min;
        float posDelta = mobCount > 1 ? (max - min) / (mobCount - 1f) : default;
        for (int i = 0; i < mobCount; i++)
        {
            var simAsset = GetSimAsset(group.Mob.Type, globalSettings);
            levelData.MobSpawns.Add(new LevelMobWaveData.MobSpawn()
            {
                MobModifierFlags = group.Mob.Modifiers,
                SimAsset = simAsset,
                Position = new Vector3(position, simAsset.TryGetComponent(out MobAuth mobAuth) ? mobAuth.SpawnHeight : 0.5f, 0),
            });
            position += posDelta;
        }
    }

    private GameObject GetSimAsset(MobPrefab mobPrefab, GlobalGameplaySettings globalSettings)
    {
        switch (mobPrefab)
        {
            case MobPrefab.Archer:
                return globalSettings.MobSEArcher;
            case MobPrefab.Bird:
                return globalSettings.MobSEBird;
            case MobPrefab.Brute:
                return globalSettings.MobSEBrute;
            case MobPrefab.BruteBoss:
                return globalSettings.MobSEBruteBoss;
            case MobPrefab.Chicken:
                return globalSettings.MobSEChicken;
            case MobPrefab.Frog:
                return globalSettings.MobSEFrog;
        }
        return null;
    }

    [System.NonSerialized]
    private LevelMobWaveData _generatedLevelData;

    public virtual void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var levelData = GetOrCreateLevelData();
        var spawnsBuffer = dstManager.AddBuffer<LevelDefinitionMobSpawn>(entity);

        foreach (var spawn in levelData.MobSpawns)
        {
            spawnsBuffer.Add(new LevelDefinitionMobSpawn()
            {
                Flags = spawn.MobModifierFlags,
                MobToSpawn = conversionSystem.GetPrimaryEntity(spawn.SimAsset.gameObject),
                Position = (fix2)(Vector2)spawn.Position,
            });
        }
    }

    public void DeclareReferencedPrefabs(List<GameObject> referencedPrefabs)
    {
        HashSet<GameObject> mobPrefabs = new HashSet<GameObject>();
        var levelData = GetOrCreateLevelData();
        foreach (var spawn in levelData.MobSpawns)
        {
            mobPrefabs.Add(spawn.SimAsset.gameObject);
        }

        referencedPrefabs.AddRange(mobPrefabs);
    }

    private LevelMobWaveData GetOrCreateLevelData()
    {
        if (_generatedLevelData == null)
        {
            _generatedLevelData = GenerateLevelData(GetComponent<LevelDefinitionAuth>().GlobalGameplaySettings);
        }

        return _generatedLevelData;
    }
}
