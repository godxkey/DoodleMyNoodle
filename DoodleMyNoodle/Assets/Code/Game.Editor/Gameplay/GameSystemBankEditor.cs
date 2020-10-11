using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorX;
using UnityEngine;


public class GameSystemBankUpdater : AssetPostprocessor
{
    const string ASSET_PATH = "Assets/ScriptableObjects/Generated/GameSystemBank.asset";

    static int s_importLoopCounter = 0;

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if (AssetPostprocessorUtility.ExitImportLoop(importedAssets, ASSET_PATH, ref s_importLoopCounter))
            return;

        var modifiedSystems = importedAssets.Where((assetPath) => assetPath.EndsWith(".prefab"))
               .Select((assetPath) => AssetDatabase.LoadAssetAtPath<GameObject>(assetPath))
               .Where((gameObject) => gameObject.GetComponent<GameSystem>() != null)
               .ToList();

        if(modifiedSystems.Count > 0)
        {
            UpdateBank(modifiedSystems);
        }
    }

    static void UpdateBank(List<GameObject> changedPrefabs)
    {
        GameSystemBank bank = AssetDatabaseX.LoadOrCreateScriptableObjectAsset<GameSystemBank>(ASSET_PATH);

        bool change = false;
        foreach (var item in changedPrefabs)
        {
            if (item.TryGetComponent(out GameSystem gameSystem))
            {
                bool isInBank = false;
                foreach (var prefab in bank.Prefabs)
                {
                    if (ReferenceEquals(prefab, gameSystem))
                    {
                        isInBank = true;
                        break;
                    }
                }

                if (!isInBank)
                {
                    bank.Prefabs.Add(gameSystem);
                    change = true;
                }
            }
        }

        if (change)
        {
            DebugEditor.LogAssetIntegrity($"GameSystem bank updated.");
            EditorUtility.SetDirty(bank);
            AssetDatabase.SaveAssets();
        }
    }

    [MenuItem("Tools/Data Management/Force Update GameSystem Bank", priority = 999)]
    static void UpdateBankComplete()
    {
        GameSystemBank bank = AssetDatabaseX.LoadOrCreateScriptableObjectAsset<GameSystemBank>(ASSET_PATH);

        bank.Prefabs.Clear();

        AssetDatabaseX.LoadPrefabAssetsWithComponentOnRoot<GameSystem>(out List<KeyValuePair<string, GameObject>> loadResult);
        foreach (KeyValuePair<string, GameObject> item in loadResult)
        {
            bank.Prefabs.Add(item.Value.GetComponent<GameSystem>());
        }

        DebugEditor.LogAssetIntegrity($"GameSystem bank updated.");
        EditorUtility.SetDirty(bank);
        AssetDatabase.SaveAssets();
    }
}