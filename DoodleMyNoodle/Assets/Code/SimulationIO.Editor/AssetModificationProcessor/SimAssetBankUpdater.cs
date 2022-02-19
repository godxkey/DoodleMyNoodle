using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorX;
using UnityEngine;
using UnityEngineX;

public class SimAssetBankUpdater : AssetPostprocessor
{
    const string ASSET_PATH = "Assets/Config/Generated/SimAssetBank.asset";

    static int s_importLoopCounter = 0;

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if (AssetPostprocessorUtility.ExitImportLoop(importedAssets, ASSET_PATH, ref s_importLoopCounter))
            return;

        SimAssetBank bank = null;
        bool saveAsset = false;
        importedAssets.Where((assetPath) => assetPath.EndsWith(".prefab"))
               .Select((assetPath) => AssetDatabase.LoadAssetAtPath<GameObject>(assetPath))
               .Where((gameObject) => gameObject.GetComponent<SimAsset>() != null)
               .Select((gameObject) => gameObject.GetComponent<SimAsset>())
               .ToList()
               .ForEach((SimAsset prefab) =>
               {
                   if (bank == null)
                   {
                       bank = AssetDatabaseX.LoadOrCreateScriptableObjectAsset<SimAssetBank>(ASSET_PATH);
                   }

                   saveAsset |= ValidateSimAssetIdForPrefab(prefab);

                   if (!bank.EditorSimAssets.Contains(prefab))
                   {
                       saveAsset = true;
                       bank.EditorSimAssets.Add(prefab);

                       DebugEditor.LogAssetIntegrity($"Added {prefab.gameObject.name} to {nameof(SimAssetBank)}.");
                   }
               });

        if (saveAsset)
        {
            EditorUtility.SetDirty(bank);
            AssetDatabase.SaveAssets();
        }
    }

    [MenuItem("Tools/Data Management/Force Update SimAsset Bank", priority = 999)]
    public static void UpdateSimAssetIds()
    {
        SimAssetBank bank = AssetDatabaseX.LoadOrCreateScriptableObjectAsset<SimAssetBank>(ASSET_PATH);
        bank.EditorSimAssets.Clear();

        AssetDatabaseX.LoadPrefabAssetsWithComponentOnRoot<SimAsset>(out List<KeyValuePair<string, GameObject>> loadResult);
        foreach (KeyValuePair<string, GameObject> item in loadResult)
        {
            var prefab = item.Value.GetComponent<SimAsset>();

            ValidateSimAssetIdForPrefab(prefab);

            bank.EditorSimAssets.Add(prefab);
        }

        DebugEditor.LogAssetIntegrity($"[{nameof(SimAssetBankUpdater)}] Updated {typeof(SimAssetBank)}");

        EditorUtility.SetDirty(bank);
        AssetDatabase.SaveAssets();
    }

    static bool ValidateSimAssetIdForPrefab(SimAsset prefab)
    {
        if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(prefab, out string guid, out long localId))
        {
            if (prefab.Guid != guid)
            {
                prefab.Editor_SetGuid(guid);
                EditorUtility.SetDirty(prefab);
                DebugEditor.LogAssetIntegrity($"[{nameof(SimAssetBankUpdater)}] Updated {prefab.name}'s assetId to: {guid}");
                PrefabUtility.RecordPrefabInstancePropertyModifications(prefab);
                return true;
            }
        }
        return false;
    }
}