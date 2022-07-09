using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngineX;

public class GlobalItemBankUpdater : AssetPostprocessor
{
    static AssetBankUpdaterPrefabComponent<GlobalItemBank, ItemAuth> s_assetBankUpdater = new AssetBankUpdaterPrefabComponent<GlobalItemBank, ItemAuth>(
        bankAssetPath: "Assets/Config/Generated/GlobalItemBank.asset",
        getStoredObjectsFromBankDelegate: (bank) => bank.Items);

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        s_assetBankUpdater.OnPostprocessAllAssets(importedAssets, deletedAssets, movedAssets, movedFromAssetPaths);
    }

    [MenuItem("Tools/Data Management/Force Update Global Item Bank", priority = 999)]
    static void UpdateBankComplete()
    {
        s_assetBankUpdater.UpdateBankComplete();
    }
}