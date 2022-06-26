using UnityEditor;

public class GlobalLevelBankUpdater : AssetPostprocessor
{
    static AssetBankUpdaterPrefabComponent<GlobalLevelBank, LevelDefinitionAuth> s_assetBankUpdater = new AssetBankUpdaterPrefabComponent<GlobalLevelBank, LevelDefinitionAuth>(
        bankAssetPath: "Assets/Config/Generated/GlobalLevelBank.asset",
        getStoredObjectsFromBankDelegate: (bank) => bank.Levels);

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        s_assetBankUpdater.OnPostprocessAllAssets(importedAssets, deletedAssets, movedAssets, movedFromAssetPaths);
    }

    [MenuItem("Tools/Data Management/Force Update Global Level Bank", priority = 999)]
    static void UpdateBankComplete()
    {
        s_assetBankUpdater.UpdateBankComplete();
    }
}

//public class GlobalAssetBankUpdater : AssetPostprocessor
//{
//    private static List<AssetBankUpdaterBase> s_assetBankUpdaters;

//    private static List<AssetBankUpdaterBase> GetAssetBankUpdaters()
//    {
//        if (s_assetBankUpdaters == null)
//        {
//            s_assetBankUpdaters = new List<AssetBankUpdaterBase>();
//            var assetBankTypes = TypeUtility.GetTypesWithAttribute(typeof(AutoAssetBankAttribute));
            
//            foreach (var assetBankType in assetBankTypes)
//            {
//                if (assetBankType.IsAbstract)
//                    continue;
//                var fields = assetBankType.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                
//                foreach (var field in fields)
//                {
//                    if (!Attribute.IsDefined(field, typeof(AutoAssetBankListAttribute)))
//                        continue;
//                    if (!field.FieldType.IsAssignableFrom(typeof(IList)))
//                        continue;
//                }
//            }
//        }

//        return s_assetBankUpdaters;
//    }

//    static AssetBankUpdaterPrefabComponent<GlobalLevelBank, LevelDefinition> s_assetBankUpdater = new AssetBankUpdaterPrefabComponent<GlobalLevelBank, LevelDefinition>(
//        bankAssetPath: "Assets/Config/Generated/GlobalLevelBank.asset",
//        getStoredObjectsFromBankDelegate: (bank) => bank.Levels);

//    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
//    {

//        List<AssetBankUpdaterBase> assetBankUpdaters = GetAssetBankUpdaters();

//        s_assetBankUpdater.OnPostprocessAllAssets(importedAssets, deletedAssets, movedAssets, movedFromAssetPaths);
//    }

//    [MenuItem("Tools/Data Management/Force Update Global Item Bank", priority = 999)]
//    static void UpdateBankComplete()
//    {
//        s_assetBankUpdater.UpdateBankComplete();
//    }
//}