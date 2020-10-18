using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditorX;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngineX;

public class GameSystemBankUpdater : AssetPostprocessor
{
    const string ASSET_PATH = "Assets/ScriptableObjects/Generated/GameSystemBank.asset";

    static int s_importLoopCounter = 0;

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
    {
        if (AssetPostprocessorUtility.ExitImportLoop(importedAssets, ASSET_PATH, ref s_importLoopCounter))
            return;

        var modifiedSystems = importedAssets.Where((assetPath) => assetPath.EndsWith(".prefab"))
               .Select((assetPath) => AssetDatabase.LoadAssetAtPath<GameObject>(assetPath))
               .Where((gameObject) => gameObject.GetComponent<GameSystem>() != null)
               .ToList();

        if (modifiedSystems.Count > 0)
        {
            UpdateBank(modifiedSystems);
        }
    }

    static void UpdateBank(List<GameObject> changedPrefabs)
    {
        GameSystemBank bank = AssetDatabaseX.LoadOrCreateScriptableObjectAsset<GameSystemBank>(ASSET_PATH);

        bool change = false;
        foreach (var item in changedPrefabs)
        {
            if (item.TryGetComponent(out GameSystem gameSystem))
            {
                bool isInBank = false;
                foreach (var prefab in bank.Prefabs)
                {
                    if (ReferenceEquals(prefab, gameSystem))
                    {
                        isInBank = true;
                        break;
                    }
                }

                if (!isInBank)
                {
                    bank.Prefabs.Add(gameSystem);
                    change = true;
                }
            }
        }

        if (change)
        {
            DebugEditor.LogAssetIntegrity($"GameSystem bank updated.");
            EditorUtility.SetDirty(bank);
            AssetDatabase.SaveAssets();
        }
    }

    [MenuItem("Tools/Data Management/Force Update GameSystem Bank", priority = 999)]
    static void UpdateBankComplete()
    {
        GameSystemBank bank = AssetDatabaseX.LoadOrCreateScriptableObjectAsset<GameSystemBank>(ASSET_PATH);

        bank.Prefabs.Clear();

        AssetDatabaseX.LoadPrefabAssetsWithComponentOnRoot<GameSystem>(out List<KeyValuePair<string, GameObject>> loadResult);
        foreach (KeyValuePair<string, GameObject> item in loadResult)
        {
            bank.Prefabs.Add(item.Value.GetComponent<GameSystem>());
        }

        DebugEditor.LogAssetIntegrity($"GameSystem bank updated.");
        EditorUtility.SetDirty(bank);
        AssetDatabase.SaveAssets();
    }

    [MenuItem("Tools/Update SE prefabs asset")]
    static void test()
    {
        foreach (GameObject prefab in AssetDatabaseX.LoadPrefabAssetsWithComponentOnRoot<ViewBindingDefinition>())
        {
            var viewGO = prefab.GetComponent<ViewBindingDefinition>().GetViewGameObject();
            var simGO = prefab.GetComponent<ViewBindingDefinition>().GetSimGameObject();

            var viewPrefab = PrefabUtility.GetCorrespondingObjectFromSource(viewGO);
            var simPrefab = PrefabUtility.GetCorrespondingObjectFromSource(simGO);

            SimAsset simAssetIdAuth = simPrefab.GetComponent<SimAsset>();
            simAssetIdAuth.Editor_SetBindedViewPrefab(viewPrefab);

            EditorUtility.SetDirty(simAssetIdAuth);
        }

        AssetDatabase.SaveAssets();
    }

    [MenuItem("Tools/Update scene")]
    static void test2()
    {
        Scene scene = EditorSceneManager.GetActiveScene();
        List<GameObject> toDestroy = new List<GameObject>();
        foreach (GameObject rootObj in scene.GetRootGameObjects())
        {
            if (rootObj.TryGetComponent(out ViewBindingDefinition viewBindingDefinition) && 
                !viewBindingDefinition.GetSimGameObject().HasComponent<LevelGridAuth>())
            {
                ViewBindingDefinition viewBindingDefinitionPrefab = PrefabUtility.GetCorrespondingObjectFromSource(viewBindingDefinition);
                GameObject simGO = viewBindingDefinitionPrefab.GetSimGameObject();
                GameObject simPrefab = PrefabUtility.GetCorrespondingObjectFromSource(simGO);
                GameObject simPrefabInstance = (GameObject)PrefabUtility.InstantiatePrefab(simPrefab, scene);
                simPrefabInstance.name = simPrefab.name;
                simPrefabInstance.transform.position = viewBindingDefinition.transform.position;


                List<ObjectOverride> overrides = PrefabUtility.GetObjectOverrides(viewBindingDefinition.gameObject);
                GameObject simSceneGO = viewBindingDefinition.GetSimGameObject();
                foreach (ObjectOverride item in overrides)
                {
                    if (item.instanceObject is Component component && component.gameObject == simSceneGO)
                    {
                        var newComponent = simPrefabInstance.GetComponent(component.GetType());
                        Copy(component, newComponent);
                        PrefabUtility.RecordPrefabInstancePropertyModifications(newComponent);
                    }
                }

                toDestroy.Add(viewBindingDefinition.gameObject);
                EditorSceneManager.MarkSceneDirty(scene);
            }
        }

        foreach (var item in toDestroy)
        {
            Object.DestroyImmediate(item);
        }
    }

    static void Copy(Object from, Object to)
    {
        if (from is MessageAuth fromMessageAuth)
        {
            ((MessageAuth)to).Value = fromMessageAuth.Value;
        }

        if(from is InventoryAuth fromInventoryAuth)
        {
            ((InventoryAuth)to).InitialItems = fromInventoryAuth.InitialItems;
            ((InventoryAuth)to).Capacity = fromInventoryAuth.Capacity;
        }
    }

    [MenuItem("Tools/Print scene")]
    static void test3()
    {
        Scene scene = EditorSceneManager.GetActiveScene();
        List<GameObject> toDestroy = new List<GameObject>();
        foreach (GameObject rootObj in scene.GetRootGameObjects())
        {
            if (rootObj && rootObj.TryGetComponent(out ViewBindingDefinition viewBindingDefinition))
            {
                GameObject simSceneGO = viewBindingDefinition.GetSimGameObject();
                foreach (ObjectOverride item in PrefabUtility.GetObjectOverrides(viewBindingDefinition.gameObject))
                {
                    if (item.instanceObject is Component component && component.gameObject == simSceneGO)
                    {
                        Log.Info($"{viewBindingDefinition.gameObject.name}: override {component.GetType().Name}");
                    }
                }
            }
        }
    }
}