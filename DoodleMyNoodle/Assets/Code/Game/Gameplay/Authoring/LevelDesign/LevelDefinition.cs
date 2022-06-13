using CCC.InspectorDisplay;
using System;
using System.Collections.Generic;
using Unity.Assertions;
using Unity.Entities;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngineX;
using UnityEngineX.InspectorDisplay;

[Flags]
public enum MobModifierFlags
{
    None = 0,
    Armored = 1 << 0,
    Brutal = 1 << 1,
    Fast = 1 << 2,
    Explosive
}

[CreateAssetMenu(menuName = "DoodleMyNoodle/Level Waves")]
public class LevelDefinition : ScriptableObject
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
        public MobModifierFlags Modifiers = MobModifierFlags.None;
    }

    [System.Serializable]
    public class MobGroup
    {
        public Vector2 Range;
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

    public float TotalEnemies = 60;
    public float WaveSpacing = 10f;
    public MobArchetype WaveSpacingMob = new MobArchetype();
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

    public LevelData GenerateLevelData(GlobalGameplaySettings globalSettings)
    {
        LastGenerationError = null;
        LevelData result = new LevelData();

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

        float waveStartingPos = 10f;
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

    private void EmplaceGroup(MobGroup group, int mobCount, float min, float max, LevelData levelData, GlobalGameplaySettings globalSettings)
    {
        float position = min;
        float posDelta = mobCount > 1 ? (max - min) / (mobCount - 1f) : default;
        for (int i = 0; i < mobCount; i++)
        {
            var simAsset = GetSimAsset(group.Mob.Type, globalSettings);
            levelData.MobSpawns.Add(new LevelData.MobSpawn()
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
}

public class LevelData
{
    public struct MobSpawn
    {
        public GameObject SimAsset;
        public MobModifierFlags MobModifierFlags;
        public Vector3 Position;
    }
    public List<MobSpawn> MobSpawns = new List<MobSpawn>();
}

[CustomEditor(typeof(LevelDefinition))]
public class LevelDefinitionEditor : Editor
{
    protected override void OnHeaderGUI()
    {
        base.OnHeaderGUI();

        if (targets != null && targets.Length == 1)
        {
            if (GUILayout.Button("Preview In Scene"))
            {
            }
            GUILayout.Space(10);
        }
    }

    private void OnEnable()
    {
        if (!Application.isPlaying && LevelDefinitionPreviewer.Instance == null)
        {
            LevelDefinitionPreviewer.CreateInstance();
            LevelDefinitionPreviewer.Instance.DisplayLevel(target as LevelDefinition);
        }
    }

    private void OnDisable()
    {
        if (LevelDefinitionPreviewer.Instance != null)
            LevelDefinitionPreviewer.DestroyInstance();
    }

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        base.OnInspectorGUI();

        if (EditorGUI.EndChangeCheck() && !Application.isPlaying)
        {
            if (LevelDefinitionPreviewer.Instance == null)
                LevelDefinitionPreviewer.CreateInstance();
            LevelDefinitionPreviewer.Instance.DisplayLevel(target as LevelDefinition);
        }
    }
}

[ExecuteInEditMode]
public class LevelDefinitionPreviewer : MonoBehaviour
{
    public static LevelDefinitionPreviewer Instance { get; private set; }

    [System.Serializable]
    public struct PrefabInstancePair
    {
        public GameObject Prefab;
        public GameObject Instance;
    }

    public List<PrefabInstancePair> SpawnedGhosts = new List<PrefabInstancePair>();

    private void OnEnable()
    {
        Instance = this;
    }

    private void OnDisable()
    {
        if (Instance == this)
            Instance = null;

        foreach (var ghost in SpawnedGhosts)
        {
            DestroyImmediate(ghost.Instance);
        }
        SpawnedGhosts.Clear();
    }

    public static void CreateInstance()
    {
        var go = new GameObject("~ Level Definition Preview ~", typeof(LevelDefinitionPreviewer));
        go.hideFlags = HideFlags.DontSave;
    }

    public static void DestroyInstance()
    {
        DestroyImmediate(Instance.gameObject);
    }

    public void DisplayLevel(LevelDefinition levelDefinition)
    {
        GlobalGameplaySettings globalSettings = GlobalGameplaySettings.GetInstance_EditorOnly();
        LevelData levelData = levelDefinition.GenerateLevelData(globalSettings);

        // spawn new children
        int i = 0;
        for (; i < levelData.MobSpawns.Count; i++)
        {
            var prefab = levelData.MobSpawns[i].SimAsset.GetComponent<SimAsset>().BindedViewPrefab;

            if (SpawnedGhosts.Count <= i || SpawnedGhosts[i].Prefab != prefab)
            {
                var ghost = Instantiate(prefab, default, Quaternion.identity, transform);
                ghost.hideFlags = HideFlags.DontSave;
                SpawnedGhosts.Insert(i, new PrefabInstancePair()
                {
                    Prefab = prefab,
                    Instance = ghost,
                });
            }

            SpawnedGhosts[i].Instance.transform.position = levelData.MobSpawns[i].Position;
        }

        // remove extra
        for (int r = SpawnedGhosts.Count - 1; r >= i; r--)
        {
            DestroyImmediate(SpawnedGhosts[r].Instance);
            SpawnedGhosts.RemoveAt(r);
        }
    }
}

[CustomPropertyDrawer(typeof(LevelDefinition.MobGroup))]
public class LevelDefinitionMobGroupDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        var propDensity = property.FindPropertyRelative(nameof(LevelDefinition.MobGroup.Density));
        var propMob = property.FindPropertyRelative(nameof(LevelDefinition.MobGroup.Mob));
        var propRange = property.FindPropertyRelative(nameof(LevelDefinition.MobGroup.Range));

        position.height = EditorGUIUtility.singleLineHeight;
        EditorGUI.BeginProperty(position, EditorGUIUtility.TrTempContent("Position"), propRange);
        if (propDensity.enumValueIndex == (int)LevelDefinition.GroupDensity.Single)
        {
            propRange.vector2Value = Vector2.one * GUI.HorizontalSlider(position, propRange.vector2Value.x, 0, 1);
        }
        else
        {
            float min = propRange.vector2Value.x;
            float max = propRange.vector2Value.y;
            EditorGUI.MinMaxSlider(position, ref min, ref max, 0, 1);
            propRange.vector2Value = new Vector2(Mathf.Min(min, max), Mathf.Max(min, max));
        }
        EditorGUI.EndProperty();

        MovePosToNextProperty();

        position.height = EditorGUI.GetPropertyHeight(propDensity);
        EditorGUI.PropertyField(position, propDensity);

        MovePosToNextProperty();

        position.height = EditorGUI.GetPropertyHeight(propMob);
        EditorGUI.PropertyField(position, propMob);

        EditorGUI.EndProperty();

        void MovePosToNextProperty() { position.y += position.height + EditorGUIUtility.standardVerticalSpacing; }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var density = property.FindPropertyRelative(nameof(LevelDefinition.MobGroup.Density));
        var mob = property.FindPropertyRelative(nameof(LevelDefinition.MobGroup.Mob));
        var range = property.FindPropertyRelative(nameof(LevelDefinition.MobGroup.Range));
        return EditorGUI.GetPropertyHeight(density) + EditorGUIUtility.standardVerticalSpacing
            + EditorGUI.GetPropertyHeight(mob) + EditorGUIUtility.standardVerticalSpacing
            + EditorGUI.GetPropertyHeight(range);
    }
}