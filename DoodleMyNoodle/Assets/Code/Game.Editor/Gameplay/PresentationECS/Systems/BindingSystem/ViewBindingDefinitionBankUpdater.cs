using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngineX;

public class ViewBindingDefinitionBankUpdater : AssetPostprocessor
{
    const string ASSET_PATH = "Assets/ScriptableObjects/Generated/ViewBindingDefinitionBank.asset";

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if (importedAssets.Contains(ASSET_PATH))
            return;

        ViewBindingDefinitionBank bank = null;
        bool setDirty = false;

        importedAssets.Where((assetPath) => assetPath.EndsWith(".prefab"))
               .Select((assetPath) => AssetDatabase.LoadAssetAtPath<GameObject>(assetPath))
               .Where((gameObject) => gameObject.HasComponent<ViewBindingDefinition>())
               .Select((gameObject) => gameObject.GetComponent<ViewBindingDefinition>())
               .ToList()
               .ForEach((prefab) =>
               {
                   if (bank == null)
                       bank = AssetDatabaseX.LoadOrCreateScriptableObjectAsset<ViewBindingDefinitionBank>(ASSET_PATH);

                   if (!bank.ViewBindingDefinitions.Contains(prefab))
                   {
                       setDirty = true;
                       bank.ViewBindingDefinitions.Add(prefab);

                       DebugEditor.LogAssetIntegrity($"Added {prefab.gameObject.name} to ViewBindingDefinitionBank.");
                   }
               });

        if(setDirty)
            EditorUtility.SetDirty(bank);
    }

    [MenuItem("Tools/Data Management/Force Update ViewBindingBank", priority = 999)]
    public static void UpdateViewBindingBank()
    {
        ViewBindingDefinitionBank bank = AssetDatabaseX.LoadOrCreateScriptableObjectAsset<ViewBindingDefinitionBank>(ASSET_PATH);

        bank.ViewBindingDefinitions.Clear();

        AssetDatabaseX.FindPrefabsAssetsWithComponent<ViewBindingDefinition>(out List<KeyValuePair<string, GameObject>> searchResult);
        foreach (KeyValuePair<string, GameObject> item in searchResult)
        {
            bank.ViewBindingDefinitions.Add(item.Value.GetComponent<ViewBindingDefinition>());
        }

        DebugEditor.LogAssetIntegrity($"ViewBindingDefinitionBank updated.");
        EditorUtility.SetDirty(bank);
    }
}