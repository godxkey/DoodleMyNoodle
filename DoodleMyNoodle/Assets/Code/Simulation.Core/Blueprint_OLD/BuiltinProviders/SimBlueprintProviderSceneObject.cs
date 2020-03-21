using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimBlueprintProviderSceneObject : MonoBehaviour, ISimBlueprintProvider
{
    Dictionary<ISceneMetaData, ISceneLoadPromise> _sceneLoads = new Dictionary<ISceneMetaData, ISceneLoadPromise>();

    public SceneMetaDataBank SceneMetaDataBank;

    public bool CanProvideBlueprintFor(in SimBlueprintId blueprintId) => blueprintId.Type == SimBlueprintId.BlueprintType.SceneGameObject;

    public bool CanProvideBlueprintSynchronously() => false;

    public SimBlueprint ProvideBlueprint(in SimBlueprintId blueprintId) => throw new NotImplementedException();


    void ISimBlueprintProvider.ProvideBlueprintBatched(in SimBlueprintId[] blueprintIds, Action<SimBlueprint[]> onComplete)
    {
        _sceneLoads.Clear();
        StartCoroutine(ProvideBlueprintsRoutine(blueprintIds, onComplete));
    }

    struct Request
    {
        public bool IsValid;
        public ISceneMetaData SceneMetaData;
        public SimBlueprintId BlueprintId;
    }

    IEnumerator ProvideBlueprintsRoutine(SimBlueprintId[] blueprintIds, Action<SimBlueprint[]> onComplete)
    {
        Request[] requests = new Request[blueprintIds.Length];
        SimBlueprint[] results = new SimBlueprint[blueprintIds.Length];


        ////////////////////////////////////////////////////////////////////////////////////////
        //      Create requests
        ////////////////////////////////////////////////////////////////////////////////////////

        for (int i = 0; i < blueprintIds.Length; i++)
        {
            if (ParseBlueprintId(blueprintIds[i], out string sceneGuid, out string gameObjectGuid) == false)
            {
                Debug.LogError($"Could not parse blueprintId {blueprintIds[i]} in SimBlueprintSceneObjectProvider");
                continue;
            }

            // find corresponding scene
            ISceneMetaData sceneMetaData = SceneMetaDataBank.SceneMetaDatas.Find((ISceneMetaData metaData) => metaData.AssetGuid == sceneGuid);

            if (sceneMetaData == null)
            {
                Debug.LogError($"Could not provide blueprint for {blueprintIds[i]} because no scene was found with the matching guid.");
                continue;
            }

            // fill request data
            requests[i].BlueprintId = blueprintIds[i];
            requests[i].SceneMetaData = sceneMetaData;
            requests[i].IsValid = true;
        }


        ////////////////////////////////////////////////////////////////////////////////////////
        //      Load all necessary scenes
        ////////////////////////////////////////////////////////////////////////////////////////
        LoadSceneParameters sceneLoadParameters = default;
        sceneLoadParameters.loadSceneMode = LoadSceneMode.Additive;
        sceneLoadParameters.localPhysicsMode = LocalPhysicsMode.None;

        for (int i = 0; i < requests.Length; i++)
        {
            if (!requests[i].IsValid)
                continue;

            // start loading scene if it's not already
            ISceneMetaData sceneMeta = requests[i].SceneMetaData;
            if (_sceneLoads.ContainsKey(sceneMeta) == false)
            {
                _sceneLoads.Add(sceneMeta, SceneService.LoadAsync(sceneMeta.Name, LoadSceneMode.Additive, LocalPhysicsMode.None));
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////
        //      Wait for all scenes to load and gather scene gameobjects
        ////////////////////////////////////////////////////////////////////////////////////////
        Dictionary<ISceneMetaData, GameObject[]> sceneRootGameObjects = new Dictionary<ISceneMetaData, GameObject[]>();
        while (sceneRootGameObjects.Count != _sceneLoads.Count)
        {
            foreach (KeyValuePair<ISceneMetaData, ISceneLoadPromise> pair in _sceneLoads)
            {
                if (pair.Value.IsComplete && !sceneRootGameObjects.ContainsKey(pair.Key))
                {
                    sceneRootGameObjects.Add(pair.Key, pair.Value.Scene.GetRootGameObjects());
                }
            }

            yield return null;
        }


        ////////////////////////////////////////////////////////////////////////////////////////
        //      Find the scene gameobject that matches each request
        ////////////////////////////////////////////////////////////////////////////////////////
        for (int i = 0; i < requests.Length; i++)
        {
            SimEntity entity = sceneRootGameObjects[requests[i].SceneMetaData].FindEntityDeepFromGameObjectGuid(requests[i].BlueprintId);
            if (entity)
            {
                results[i] = new SimBlueprint(requests[i].BlueprintId, entity);
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////
        //      Return result
        ////////////////////////////////////////////////////////////////////////////////////////
        onComplete(results);
    }


    void ISimBlueprintProvider.ReleaseBatchedBlueprints()
    {
        foreach (var item in _sceneLoads)
        {
            if (item.Value.Scene.isLoaded)
            {
                SceneService.UnloadAsync(item.Value.Scene);
            }
        }

        _sceneLoads.Clear();
    }



    #region Static Methods
    public static bool ParseBlueprintId(in SimBlueprintId blueprintId, out string sceneGuid, out string gameObjectGuid)
    {
        string stringData = blueprintId.Value;
        if (!stringData.IsNullOrEmpty())
        {
            int indexOfSplit = stringData.IndexOf('/');
            gameObjectGuid = (indexOfSplit > 0) ? stringData.Remove(stringData.IndexOf('/')) : "";
            sceneGuid = (indexOfSplit < stringData.Length - 1) ? stringData.Substring(stringData.IndexOf('/') + 1) : "";

            return true;
        }
        else
        {
            sceneGuid = "";
            gameObjectGuid = "";

            return false;
        }
    }
    public static bool CompareGameObjectGuid(in SimBlueprintId a, in SimBlueprintId b)
    {
        int compareStringUpTo = a.Value.IndexOf('/');
        if (b.Value.IndexOf('/') != compareStringUpTo)
            return false;

        for (int i = 0; i < compareStringUpTo; i++)
        {
            if (a.Value[i] != b.Value[i])
            {
                return false;
            }
        }

        return true;
    }

    public static SimBlueprintId MakeBlueprintId(string sceneGuid, string gameObjectGuid)
    {
        return new SimBlueprintId(SimBlueprintId.BlueprintType.SceneGameObject, $"{gameObjectGuid}/{sceneGuid}");
    }
    #endregion
}
