using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad] // to make sure the static constructor is called
public class SceneBlueprintIdUpdater : UnityEditor.AssetModificationProcessor
{
    static SceneBlueprintIdUpdater() // static constructor called at the very start
    {
        SimModuleSceneLoader.s_EditorValidationMethod = InjectSimBlueprintIdsOnSceneGameObjects;
    }

    //const string ASSET_PATH = "Assets/ScriptableObjects/SimBlueprintProviders/SceneBlueprintIdInjectorData.asset";
    const string SCENE_ASSET_EXTENSION = ".unity";

    static string[] OnWillSaveAssets(string[] paths)
    {
        paths.Where((assetPath) => assetPath.EndsWith(SCENE_ASSET_EXTENSION))
            .Select((scenePath) => EditorSceneManager.GetSceneByPath(scenePath))
            .ToList()
            .ForEach((scene) => InjectSimBlueprintIdsOnSceneGameObjects(scene));

        return paths;
    }

    static void InjectSimBlueprintIdsOnSceneGameObjects(Scene scene)
    {
        string newSceneGuid = GetSceneGuid(scene);
        //SceneBlueprintIdInjectorData dataBank = EditorHelpers.GetOrCreateDataAsset<SceneBlueprintIdInjectorData>(ASSET_PATH);
        //SceneBlueprintIdInjectorData.SceneData sceneData = dataBank.SceneDatas.Find((x) => x.SceneAssetGuid == sceneGuid);

        //if (sceneData == null)
        //{
        //    sceneData = new SceneBlueprintIdInjectorData.SceneData();
        //    sceneData.SceneAssetGuid = sceneGuid;
        //    dataBank.SceneDatas.Add(sceneData);
        //}

        List<SimEntity> simEntities = GetAllSimEntitiesInScene(scene);


        // Very simple id generator. First entity gets 0, second entity gets 1, third entity gets 2 ...
        // Modifying the scene even slightly could change all of the id assignements. This means we would not be able
        // to correctly load a past saved game with an updated scene. This id generator might need to be reworked into something more robust.
        // For example, making a unique hash out of the entity name. The best would be to use Unity's internal fileLocalId but it's not easily
        // accessible and the API sucks.
        uint newId = 0;

        foreach (SimEntity entity in simEntities)
        {
            // a SceneGameObject's blueprint is composed of 2 parts:
            // 1. The scene guid
            // 2. The object guid

            if (!IsBlueprintIdUpToDate(entity.BlueprintId, newSceneGuid, newId))
            {
                SimBlueprintId newBlueprintId = SimBlueprintProviderSceneObject.MakeBlueprintId(newSceneGuid, newId.ToString());

                entity.BlueprintId = newBlueprintId;

                if (Application.isPlaying)
                {
                    DebugService.LogError($"Updating {entity.gameObject.name}'s blueprintId to {entity.BlueprintId}. " +
                        $"This should not happen at runtime. Try resaving your scene.");
                }
                else
                {
                    if (PrefabUtility.IsPartOfAnyPrefab(entity))
                    {
                        PrefabUtility.RecordPrefabInstancePropertyModifications(entity);
                    }

                    DebugEditor.LogAssetIntegrity($"Updating {entity.gameObject.name}'s blueprintId to {entity.BlueprintId}");
                }
            }


            newId++;
        }

        //EditorUtility.SetDirty(dataBank);
    }

    static void ValidateSceneBlueprintsIds(Scene scene)
    {
        string newSceneGuid = GetSceneGuid(scene);
        List<SimEntity> simEntities = GetAllSimEntitiesInScene(scene);
        uint newId = 0;
        foreach (SimEntity entity in simEntities)
        {
            // a SceneGameObject's blueprint is composed of 2 parts:
            // 1. The scene guid
            // 2. The object guid

            if (!IsBlueprintIdUpToDate(entity.BlueprintId, newSceneGuid, newId))
            {
                SimBlueprintId newBlueprintId = SimBlueprintProviderSceneObject.MakeBlueprintId(newSceneGuid, newId.ToString());

                entity.BlueprintId = newBlueprintId;

                if (PrefabUtility.IsPartOfAnyPrefab(entity))
                {
                    PrefabUtility.RecordPrefabInstancePropertyModifications(entity);
                }

                DebugEditor.LogAssetIntegrity($"Updating {entity.gameObject.name}'s blueprintId to {entity.BlueprintId}");
            }


            newId++;
        }
    }

    static bool IsBlueprintIdUpToDate(SimBlueprintId simBlueprintId, string newSceneId, uint newGameObjectId)
    {
        // a SceneGameObject's blueprint is composed of 2 parts:
        // 1. The scene guid
        // 2. The object guid
        SimBlueprintProviderSceneObject.ParseBlueprintId(simBlueprintId, out string oldSceneGuid, out string oldGameObjecGuid);
        if (oldSceneGuid != newSceneId)
        {
            return false;
        }

        if (uint.TryParse(oldGameObjecGuid, out uint oldGameObjectGuidNumber))
        {
            if (newGameObjectId != oldGameObjectGuidNumber)
            {
                return false;
            }
        }
        else
        {
            return false;
        }

        return true;
    }

    static List<SimEntity> GetAllSimEntitiesInScene(Scene scene)
    {
        List<SimEntity> simEntities = new List<SimEntity>();
        scene.GetRootGameObjects()
            .ToList()
            .ForEach((rootGameObject) =>
            {
                SimEntity simEntity = rootGameObject.GetComponent<SimEntity>();
                if (simEntity != null)
                {
                    simEntities.AddRange(rootGameObject.GetComponentsInChildren<SimEntity>());
                }
            });

        return simEntities;
    }

    static string GetSceneGuid(Scene scene)
    {
        return AssetDatabase.AssetPathToGUID(scene.path);
    }
}