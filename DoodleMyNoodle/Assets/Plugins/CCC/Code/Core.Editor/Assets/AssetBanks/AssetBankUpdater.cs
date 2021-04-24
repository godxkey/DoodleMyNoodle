using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorX;
using UnityEngine;
using UnityEngineX;

public class AssetBankUpdater<TBank, TAsset, TStoredObject>
    where TBank : ScriptableObject
    where TAsset : UnityEngine.Object
    where TStoredObject : UnityEngine.Object
{
    int _importLoopCounter = 0;

    public delegate bool EvaluateAssetDelegate(TAsset asset, out TStoredObject outStoredObject);
    public delegate List<TStoredObject> GetStoredObjectsFromBankDelegate(TBank bank);

    private string _bankAssetPath;
    private string _assetTypeFileExtension;
    private string _assetSearchType;
    private EvaluateAssetDelegate _evaluateAssetDelegate;
    private GetStoredObjectsFromBankDelegate _getStoredObjectsFromBankDelegate;

    public AssetBankUpdater(
        string bankAssetPath,
        string assetTypeFileExtension,
        string assetSearchType,
        EvaluateAssetDelegate evaluateAssetDelegate,
        GetStoredObjectsFromBankDelegate getStoredObjectsFromBankDelegate)
    {
        _bankAssetPath = bankAssetPath;
        _assetTypeFileExtension = assetTypeFileExtension;
        _assetSearchType = assetSearchType;
        _evaluateAssetDelegate = evaluateAssetDelegate;
        _getStoredObjectsFromBankDelegate = getStoredObjectsFromBankDelegate;
    }

    public void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if (AssetPostprocessorUtility.ExitImportLoop(importedAssets, _bankAssetPath, ref _importLoopCounter))
            return;

        List<TStoredObject> modifiedItems = new List<TStoredObject>();

        importedAssets.Where((assetPath) => assetPath.EndsWith(_assetTypeFileExtension))
               .Select((assetPath) => AssetDatabase.LoadAssetAtPath<TAsset>(assetPath))
               .ToList().ForEach((assetObject) =>
               {
                   if (_evaluateAssetDelegate(assetObject, out TStoredObject item))
                   {
                       modifiedItems.Add(item);
                   }
               });

        if (modifiedItems.Count > 0)
        {
            UpdateBank(modifiedItems);
        }
    }

    private void UpdateBank(List<TStoredObject> changedPrefabs)
    {
        TBank bank = AssetDatabaseX.LoadOrCreateAsset(_bankAssetPath, () => ScriptableObject.CreateInstance<TBank>());

        var storedObjects = _getStoredObjectsFromBankDelegate(bank);

        bool change = false;
        foreach (var item in changedPrefabs)
        {
            if (!storedObjects.Contains(item))
            {
                storedObjects.Add(item);
                change = true;
            }
        }

        if (change)
        {
            DebugEditor.LogAssetIntegrity($"{bank.name} updated.");
            EditorUtility.SetDirty(bank);
            AssetDatabase.SaveAssets();
        }
    }

    public void UpdateBankComplete()
    {
        TBank bank = AssetDatabaseX.LoadOrCreateAsset(_bankAssetPath, () => ScriptableObject.CreateInstance<TBank>());

        var storedObjects = _getStoredObjectsFromBankDelegate(bank);
        storedObjects.Clear();

        LoadAll(out List<(string, TAsset)> loadResult);

        foreach ((string guid, TAsset asset) in loadResult)
        {
            if (_evaluateAssetDelegate(asset, out TStoredObject storedObject))
            {
                storedObjects.Add(storedObject);
            }
        }

        DebugEditor.LogAssetIntegrity($"{bank.name} bank updated.");
        EditorUtility.SetDirty(bank);
        AssetDatabase.SaveAssets();
    }

    private void LoadAll(out List<(string, TAsset)> result)
    {
        string[] guids = AssetDatabase.FindAssets("t:" + _assetSearchType);

        result = new List<(string, TAsset)>(guids.Length);

        for (int i = 0; i < guids.Length; i++)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guids[i]);
            var asset = AssetDatabase.LoadAssetAtPath<TAsset>(assetPath);
            if (asset != null)
            {
                result.Add((guids[i], asset));
            }
        }
    }
}
