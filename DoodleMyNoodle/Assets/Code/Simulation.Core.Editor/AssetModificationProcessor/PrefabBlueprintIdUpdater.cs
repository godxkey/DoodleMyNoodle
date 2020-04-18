using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class PrefabBlueprintIdUpdater : AssetPostprocessor
{
    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        importedAssets.Where((assetPath) => assetPath.EndsWith(".prefab"))
               .Select((assetPath) => AssetDatabase.LoadAssetAtPath<GameObject>(assetPath))
               .Where((gameObject) => gameObject.GetComponent<SimEntity>() != null)
               .ToList()
               .ForEach((prefab) => ValidateBlueprintIdForPrefab(prefab));
    }

    [MenuItem("Tools/Data Management/Force Update Prefab BlueprintIds", priority = 999)]
    public static void UpdateBlueprintBank()
    {
        AssetDatabaseX.FindPrefabsAssetsWithComponent<SimEntity>(out List<KeyValuePair<string, GameObject>> searchResult);
        foreach (KeyValuePair<string, GameObject> item in searchResult)
        {
            ValidateBlueprintIdForPrefab(item.Value.gameObject);
        }
    }

    static void ValidateBlueprintIdForPrefab(GameObject prefab)
    {
        if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(prefab, out string guid, out long localId))
        {
            SimEntity entity = prefab.GetComponent<SimEntity>();

            SimBlueprintId newId = SimBlueprintProviderPrefab.MakeBlueprintId(guid);

            if (entity.BlueprintId != newId)
            {
                entity.BlueprintId = newId;
                DebugEditor.LogAssetIntegrity($"updated {prefab.name}'s blueprintId to: {newId}");


                PrefabUtility.RecordPrefabInstancePropertyModifications(prefab);
                EditorUtility.SetDirty(prefab);
            }

        }
    }
}
