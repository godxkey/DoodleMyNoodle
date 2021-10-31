using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class LevelBankUpdater : AssetPostprocessor
{
    static AssetBankUpdaterScriptableObject<LevelBank, Level> s_assetBankUpdater = new AssetBankUpdaterScriptableObject<LevelBank, Level>(
        bankAssetPath: "Assets/Config/Generated/LevelBank.asset",
        getStoredObjectsFromBankDelegate: (bank) => bank.Levels);

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        s_assetBankUpdater.OnPostprocessAllAssets(importedAssets, deletedAssets, movedAssets, movedFromAssetPaths);
    }

    [MenuItem("Tools/Data Management/Force Update Level Bank", priority = 999)]
    static void UpdateBankComplete()
    {
        s_assetBankUpdater.UpdateBankComplete();
    }
}