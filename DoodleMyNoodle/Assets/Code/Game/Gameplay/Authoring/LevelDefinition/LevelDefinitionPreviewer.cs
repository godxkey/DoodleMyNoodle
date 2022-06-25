#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;

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
                ghost.AddComponent<LevelDefinitionPreviewMobGizmos>();
                SpawnedGhosts.Insert(i, new PrefabInstancePair()
                {
                    Prefab = prefab,
                    Instance = ghost,
                });
            }

            SpawnedGhosts[i].Instance.transform.position = levelData.MobSpawns[i].Position;
            SpawnedGhosts[i].Instance.GetComponent<LevelDefinitionPreviewMobGizmos>().MobModifierFlags = levelData.MobSpawns[i].MobModifierFlags;
        }

        // remove extra
        for (int r = SpawnedGhosts.Count - 1; r >= i; r--)
        {
            DestroyImmediate(SpawnedGhosts[r].Instance);
            SpawnedGhosts.RemoveAt(r);
        }
    }
}
#endif