using UnityEngine;

public class PrefabComponentAssetBankUpdater<TBank, TPrefabComponent> : AssetBankUpdater<TBank, GameObject, TPrefabComponent>
    where TBank : ScriptableObject
    where TPrefabComponent : UnityEngine.Object
{
    public PrefabComponentAssetBankUpdater(string bankAssetPath, GetStoredObjectsFromBankDelegate getStoredObjectsFromBankDelegate)
        : base(
            bankAssetPath: bankAssetPath,
            assetTypeFileExtension: ".prefab",
            assetSearchType: "prefab",
            evaluateAssetDelegate: EvaluateAsset,
            getStoredObjectsFromBankDelegate: getStoredObjectsFromBankDelegate)
    {
    }

    private static bool EvaluateAsset(GameObject asset, out TPrefabComponent outStoredObject)
    {
        if (asset == null)
        {
            outStoredObject = null;
            return false;
        }

        return asset.TryGetComponent(out outStoredObject);
    }
}
