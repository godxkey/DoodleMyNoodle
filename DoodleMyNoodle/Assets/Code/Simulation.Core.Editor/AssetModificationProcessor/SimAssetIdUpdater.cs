using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class SimAssetIdUpdater : AssetPostprocessor
{
    const string ASSET_PATH = "Assets/ScriptableObjects/Generated/SimAssetIdMap.asset";
    
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if (importedAssets.Contains(ASSET_PATH))
            return;

        bool thereWasAChange = false;
        importedAssets.Where((assetPath) => assetPath.EndsWith(".prefab"))
               .Select((assetPath) => AssetDatabase.LoadAssetAtPath<GameObject>(assetPath))
               .Where((gameObject) => gameObject.GetComponent<SimAssetIdAuth>() != null)
               .ToList()
               .ForEach((prefab) => thereWasAChange |= ValidateSimAssetIdForPrefab(prefab));

        if (thereWasAChange)
        {
            UpdateSimAssetIdMap();
        }
    }

    [MenuItem("Tools/Data Management/Force Update Prefab SimAssetIds", priority = 999)]
    public static void UpdateSimAssetIds()
    {
        AssetDatabaseX.FindPrefabsAssetsWithComponent<SimAssetIdAuth>(out List<KeyValuePair<string, GameObject>> searchResult);
        foreach (KeyValuePair<string, GameObject> item in searchResult)
        {
            ValidateSimAssetIdForPrefab(item.Value.gameObject);
        }

        UpdateSimAssetIdMap();
    }

    static bool ValidateSimAssetIdForPrefab(GameObject prefab)
    {
        if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(prefab, out string guid, out long localId))
        {
            SimAssetIdAuth assetIdAuth = prefab.GetComponent<SimAssetIdAuth>();

            if (assetIdAuth.Guid != guid)
            {
                assetIdAuth.Guid = guid;
                DebugEditor.LogAssetIntegrity($"updated {prefab.name}'s assetId to: {guid}");


                PrefabUtility.RecordPrefabInstancePropertyModifications(prefab);
                EditorUtility.SetDirty(prefab);
                return true;
            }
        }
        return false;
    }


    static void UpdateSimAssetIdMap()
    {
        SimAssetIdMap map = AssetDatabaseX.LoadOrCreateScriptableObjectAsset<SimAssetIdMap>("Assets/ScriptableObjects/Generated/SimAssetIdMap.asset");

        map.EditorGuids.Clear();
        
        AssetDatabaseX.FindPrefabsAssetsWithComponent<SimAssetIdAuth>(out List<KeyValuePair<string, GameObject>> searchResult);
        foreach (KeyValuePair<string, GameObject> item in searchResult)
        {
            if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(item.Value, out string guid, out long localId))
            {
                map.EditorGuids.Add(guid);
            }
        }

        DebugEditor.LogAssetIntegrity($"SimAssetId map updated.");
        EditorUtility.SetDirty(map);
    }
}