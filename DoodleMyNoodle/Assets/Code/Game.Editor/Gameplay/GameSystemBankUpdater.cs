using System;
using UnityEditor;
using UnityEngine;

public class GameSystemBankUpdater : AssetPostprocessor
{
    static PrefabComponentAssetBankUpdater<GameSystemBank, GameSystem> s_assetBankUpdater = new PrefabComponentAssetBankUpdater<GameSystemBank, GameSystem>(
        bankAssetPath: "Assets/Config/Generated/GameSystemBank.asset",
        getStoredObjectsFromBankDelegate: (bank) => bank.Prefabs);

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        s_assetBankUpdater.OnPostprocessAllAssets(importedAssets, deletedAssets, movedAssets, movedFromAssetPaths);
    }

    [MenuItem("Tools/Data Management/Force Update GameSystem Bank", priority = 999)]
    static void UpdateBankComplete()
    {
        s_assetBankUpdater.UpdateBankComplete();
    }
}
