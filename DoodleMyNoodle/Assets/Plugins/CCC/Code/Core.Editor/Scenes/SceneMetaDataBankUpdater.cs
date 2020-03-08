using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SceneMetaDataBankUpdater : AssetPostprocessor
{
    const string ASSET_PATH = "Assets/ScriptableObjects/SceneMetaData/SceneMetaDataBank.asset";

    [MenuItem("Tools/Data Management/Force Update Scene Meta-data Bank", priority = 999)]
    static void UpdateMetaData()
    {
        UpdateMetaData(LogMode.Full);
    }

    enum LogMode
    {
        Silent,
        ChangesOnly,
        Full
    }

    static void UpdateMetaData(LogMode logMode)
    {
        SceneMetaDataBank dataAsset = AssetDatabaseX.LoadOrCreateScriptableObjectAsset<SceneMetaDataBank>(ASSET_PATH);

        if (dataAsset == null)
        {
            Debug.LogWarning($"Could not update SceneMetaDataBank. None found at [{ASSET_PATH}]");
            return;
        }

        List<SceneMetaDataBank.SceneMetaData> oldData = dataAsset.SceneMetaDatasInternal;
        List<SceneMetaDataBank.SceneMetaData> newData = new List<SceneMetaDataBank.SceneMetaData>();

        int buildIndex = 0;
        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            SceneMetaDataBank.SceneMetaData metaData = new SceneMetaDataBank.SceneMetaData();

            metaData.AssetGuid = scene.guid.ToString();
            metaData.Path = scene.path;
            metaData.Name = AssetDatabaseX.GetAssetNameFromPath(scene.path);
            metaData.BuildIndex = buildIndex;

            newData.Add(metaData);
            buildIndex++;
        }

        dataAsset.SceneMetaDatasInternal = newData;


        // fbessette this diff algo could be optimized
        if(logMode == LogMode.Full || logMode == LogMode.ChangesOnly)
        {
            if (oldData != null)
            {
                for (int i = 0; i < oldData.Count; i++)
                {
                    if (newData.FindIndex((x) => x.AssetGuid == oldData[i].AssetGuid) == -1)
                    {
                        DebugEditor.LogAssetIntegrity($"<color=red>Removed scene meta-data:</color> {oldData[i].Name}");
                    }
                }
                for (int i = 0; i < newData.Count; i++)
                {
                    int oldDataIndex = oldData.FindIndex((x) => x.AssetGuid == newData[i].AssetGuid);
                    if (oldDataIndex == -1)
                    {
                        DebugEditor.LogAssetIntegrity($"<color=green>Added scene meta-data:</color> {newData[i].Name}");
                    }
                    else if (oldData[oldDataIndex].ContentEquals(newData[i]) == false)
                    {
                        DebugEditor.LogAssetIntegrity($"<color=green>Updated scene meta-data:</color> {newData[i].Name}");
                    }
                }
            }
            else
            {
                for (int i = 0; i < newData.Count; i++)
                {
                    DebugEditor.LogAssetIntegrity($"<color=green>Added scene meta-data:</color> {newData[i].Name}");
                }
            }
        }

        if(logMode == LogMode.Full)
        {
            DebugEditor.LogAssetIntegrity("Scene meta-data bank updated");
        }

        EditorUtility.SetDirty(dataAsset);
    }


    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if (importedAssets.Contains(ASSET_PATH))
            return;

        System.Predicate<string> sceneAssetPath = (x) => x.EndsWith(".unity");

        if(importedAssets.Contains(sceneAssetPath)
            || deletedAssets.Contains(sceneAssetPath)
            || movedAssets.Contains(sceneAssetPath))
        {
            UpdateMetaData(LogMode.ChangesOnly);
        }
    }
}