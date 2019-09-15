using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SimBlueprintBankUpdater
{
    const string ASSET_PATH = "Assets/ScriptableObjects/BlueprintBank/SimBlueprintBank.asset";

    static SimBlueprintBank GetBlueprintBank()
    {
        return AssetDatabase.LoadAssetAtPath<SimBlueprintBank>(ASSET_PATH);
    }

    [MenuItem("Tools/Update Sim Blueprint Bank")]
    public static void UpdateBlueprintBank()
    {
        AssetDatabase.Refresh();

        SimBlueprintBank blueprintBank = GetBlueprintBank();

        if (blueprintBank == null)
        {
            Debug.LogWarning($"Could not update sim blueprint bank. None found at [{ASSET_PATH}]");
            return;
        }



        List<KeyValuePair<string, GameObject>> searchResult;
        AssetDatabaseX.FindPrefabsAssetsWithComponent<SimEntity>(out searchResult);


        List<SimBlueprintBank.PrefabData> oldData = blueprintBank.PrefabData_Editor;
        List<SimBlueprintBank.PrefabData> newData = new List<SimBlueprintBank.PrefabData>(searchResult.Count);

        for (int i = 0; i < searchResult.Count; i++)
        {
            SimBlueprintId id = SimBlueprintUtility.GetSimBlueprintIdFromBakedPrefabGUID(searchResult[i].Key);
            SimEntity prefab = searchResult[i].Value.GetComponent<SimEntity>();

            if (prefab)
            {
                newData.Add(new SimBlueprintBank.PrefabData()
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


        blueprintBank.PrefabData_Editor = newData;

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
