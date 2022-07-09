using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class MapBankUpdater : AssetPostprocessor
{
    static AssetBankUpdaterScriptableObject<MapBank, Map> s_assetBankUpdater = new AssetBankUpdaterScriptableObject<MapBank, Map>(
        bankAssetPath: "Assets/Config/Generated/MapBank.asset",
        getStoredObjectsFromBankDelegate: (bank) => bank.Maps);

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        s_assetBankUpdater.OnPostprocessAllAssets(importedAssets, deletedAssets, movedAssets, movedFromAssetPaths);
    }

    [MenuItem("Tools/Data Management/Force Update Map Bank", priority = 999)]
    static void UpdateBankComplete()
    {
        s_assetBankUpdater.UpdateBankComplete();
    }
}