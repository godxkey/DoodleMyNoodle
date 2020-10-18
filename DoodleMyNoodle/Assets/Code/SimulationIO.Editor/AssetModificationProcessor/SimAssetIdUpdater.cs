using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorX;
using UnityEngine;
using UnityEngineX;

public class SimAssetIdUpdater : AssetPostprocessor
{
    const string ASSET_PATH = "Assets/ScriptableObjects/Generated/SimAssetIdMap.asset";

    static int s_importLoopCounter = 0;

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if (AssetPostprocessorUtility.ExitImportLoop(importedAssets, ASSET_PATH, ref s_importLoopCounter))
            return;

        bool thereWasAChange = false;
        importedAssets.Where((assetPath) => assetPath.EndsWith(".prefab"))
               .Select((assetPath) => AssetDatabase.LoadAssetAtPath<GameObject>(assetPath))
               .Where((gameObject) => gameObject.GetComponent<SimAsset>() != null)
               .ToList()
               .ForEach((prefab) => thereWasAChange |= ValidateSimAssetIdForPrefab(prefab));

        if (thereWasAChange)
        {
            UpdateSimAssetIdMap();
            Log.Info($"SimAssetIdUpdater: SaveAssets");
            AssetDatabase.SaveAssets();
        }
    }

    [MenuItem("Tools/Data Management/Force Update SimAssetIds", priority = 999)]
    public static void UpdateSimAssetIds()
    {
        AssetDatabaseX.LoadPrefabAssetsWithComponentOnRoot<SimAsset>(out List<KeyValuePair<string, GameObject>> loadResult);
        foreach (KeyValuePair<string, GameObject> item in loadResult)
        {
            ValidateSimAssetIdForPrefab(item.Value.gameObject);
        }

        UpdateSimAssetIdMap();
        AssetDatabase.SaveAssets();
    }

    static bool ValidateSimAssetIdForPrefab(GameObject prefab)
    {
        Log.Info($"SimAssetIdUpdater: ValidateSimAssetIdForPrefab({prefab.name})");
        if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(prefab, out string guid, out long localId))
        {
            SimAsset assetIdAuth = prefab.GetComponent<SimAsset>();

            if (assetIdAuth.Guid != guid)
            {
                Log.Info($"SimAssetIdUpdater: ValidateSimAssetIdForPrefab -> there was a change");
                assetIdAuth.Editor_SetGuid(guid);
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
        Log.Info($"SimAssetIdUpdater: UpdateSimAssetIdMap");
        SimAssetIdMap map = AssetDatabaseX.LoadOrCreateScriptableObjectAsset<SimAssetIdMap>(ASSET_PATH);

        map.EditorGuids.Clear();

        AssetDatabaseX.LoadPrefabAssetsWithComponentOnRoot<SimAsset>(out List<KeyValuePair<string, GameObject>> loadResult);
        foreach (KeyValuePair<string, GameObject> item in loadResult)
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