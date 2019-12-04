using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SimPrefabBankUpdater : AssetPostprocessor
{
    const string ASSET_PATH = "Assets/ScriptableObjects/SimBlueprintProviders/SimPrefabBank.asset";

    // Disabled for now because it causes a lag spike in the editor. Could probably be optimized and re-enabled

    //static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    //{
    //    if (importedAssets.Contains((assetPath) => assetPath.EndsWith(".prefab")) ||
    //        deletedAssets.Contains((assetPath) => assetPath.EndsWith(".prefab")))
    //    {
    //        UpdateBlueprintBank();
    //    }
    //}

    [MenuItem("Tools/Data Management/Update Sim Prefab Bank")]
    public static void UpdateBlueprintBank()
    {
        AssetDatabase.Refresh();

        SimPrefabBank blueprintBank = AssetDatabaseX.LoadOrCreateScriptableObjectAsset<SimPrefabBank>(ASSET_PATH);

        List<KeyValuePair<string, GameObject>> searchResult;
        AssetDatabaseX.FindPrefabsAssetsWithComponent<SimEntity>(out searchResult);


        List<SimPrefabBank.PrefabData> oldData = blueprintBank._PrefabBlueprints;
        List<SimPrefabBank.PrefabData> newData = new List<SimPrefabBank.PrefabData>(searchResult.Count);

        for (int i = 0; i < searchResult.Count; i++)
        {
            SimBlueprintId id = SimBlueprintProviderPrefab.MakeBlueprintId(prefabGuid: searchResult[i].Key);
            SimEntity prefab = searchResult[i].Value.GetComponent<SimEntity>();

            if (prefab)
            {
                newData.Add(new SimPrefabBank.PrefabData()
                {
                    BlueprintId = id,
                    Prefab = prefab,
                    EntityName = prefab.gameObject.name
                });
            }
            else
            {
                Debug.LogError("Wat ? Bug here!");
            }
        }


        blueprintBank._PrefabBlueprints = newData;

        // fbessette this diff algo could be optimized
        for (int i = 0; i < oldData.Count; i++)
        {
            if (newData.FindIndex((x) => x.BlueprintId == oldData[i].BlueprintId) == -1)
            {
                Debug.Log($"<color=red>Removed blueprint prefab:</color> {oldData[i].EntityName}");
            }
        }
        for (int i = 0; i < newData.Count; i++)
        {
            if (oldData.FindIndex((x) => x.BlueprintId == newData[i].BlueprintId) == -1)
            {
                Debug.Log($"<color=green>Added blueprint prefab:</color> {newData[i].EntityName}");
            }
        }
        Debug.Log($"Blueprint bank updated ✓");

        EditorUtility.SetDirty(blueprintBank);

        AssetDatabase.Refresh();
    }
}
