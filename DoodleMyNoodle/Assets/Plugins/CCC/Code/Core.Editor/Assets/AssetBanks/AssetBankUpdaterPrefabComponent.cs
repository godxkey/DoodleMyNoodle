using UnityEngine;

public class AssetBankUpdaterPrefabComponent<TBank, TPrefabComponent> : AssetBankUpdater<TBank, GameObject, TPrefabComponent>
    where TBank : ScriptableObject
    where TPrefabComponent : Component
{
    public AssetBankUpdaterPrefabComponent(string bankAssetPath, GetStoredObjectsFromBankDelegate getStoredObjectsFromBankDelegate)
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
            outStoredObject = default;
            return false;
        }

        return asset.TryGetComponent(out outStoredObject);
    }
}
