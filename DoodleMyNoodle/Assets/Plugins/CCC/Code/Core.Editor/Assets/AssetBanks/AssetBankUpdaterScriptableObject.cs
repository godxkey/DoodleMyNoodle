using UnityEngine;

public class AssetBankUpdaterScriptableObject<TBank, TScriptableObject> : AssetBankUpdater<TBank, TScriptableObject, TScriptableObject>
    where TBank : ScriptableObject
    where TScriptableObject : ScriptableObject
{
    public AssetBankUpdaterScriptableObject(string bankAssetPath, GetStoredObjectsFromBankDelegate getStoredObjectsFromBankDelegate)
        : base(
            bankAssetPath: bankAssetPath,
            assetTypeFileExtension: ".asset",
            assetSearchType: "scriptableObject",
            evaluateAssetDelegate: EvaluateAsset,
            getStoredObjectsFromBankDelegate: getStoredObjectsFromBankDelegate)
    {
    }

    private static bool EvaluateAsset(TScriptableObject asset, out TScriptableObject outStoredObject)
    {
        outStoredObject = asset;
        return outStoredObject != null;
    }
}
