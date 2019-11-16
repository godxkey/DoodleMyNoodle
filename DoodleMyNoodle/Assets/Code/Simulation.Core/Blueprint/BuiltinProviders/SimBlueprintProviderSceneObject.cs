using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimBlueprintProviderSceneObject : MonoBehaviour, ISimBlueprintProvider
{
    public SceneMetaDataBank SceneMetaDataBank;

    public bool CanProvideBlueprintFor(in SimBlueprintId blueprintId) => blueprintId.Type == SimBlueprintId.BlueprintType.SceneGameObject;

    public bool CanProvideBlueprintSynchronously() => false;

    public SimBlueprint ProvideBlueprint(in SimBlueprintId blueprintId) => throw new NotImplementedException();




    struct BlueprintRequest
    {
        public string SceneName;
        public string GameObjectId;
        public SimBlueprint Result;
    }
    List<BlueprintRequest> _blueprintRequests = new List<BlueprintRequest>();
    bool _isInProvidingProcess = false;


    public void BeginProvideBlueprint()
    {
        _isInProvidingProcess = true;
        _blueprintRequests.Clear();
    }

    public void ProvideBlueprintAsync(in SimBlueprintId blueprintId, Action<SimBlueprint> onComplete)
    {
        if (!ParseBlueprintId(blueprintId, out string sceneGuid, out string gameObjectGuid))
        {
            Debug.LogError($"Could not parse blueprintId {blueprintId} in SimBlueprintSceneObjectProvider");
            onComplete(new SimBlueprint());
            return;
        }

        // find corresponding scene
        ISceneMetaData sceneMetaData = SceneMetaDataBank.SceneMetaDatas.Find((ISceneMetaData metaData) => metaData.AssetGuid == sceneGuid);

        if (sceneMetaData == null)
        {
            Debug.LogError($"Could not provide blueprint for {blueprintId} because no scene was found with the matching guid.");
            onComplete(new SimBlueprint());
            return;
        }


        LoadSceneParameters loadParamerters = default;
        loadParamerters.loadSceneMode = LoadSceneMode.Additive;
        loadParamerters.localPhysicsMode = LocalPhysicsMode.None;


        //SceneManager.LoadSceneAsync(blueprintId.Value)
    }



    public void ProvideBlueprintsAsync(in List<SimBlueprintId> requests, Action<List<SimBlueprint>> onComplete)
    {
        if (SceneMetaDataBank == null)
        {
            Debug.LogError($"Cannot provide blueprints because the SceneMetaDataBank reference is null");
            onComplete(null);
            return;
        }


    }


    struct Request
    {
        public ISceneMetaData SceneMetaData;
        public string GameObjectGuid;
        public int Index;
    }

    IEnumerator ProvideBlueprintsRoutine(List<SimBlueprintId> blueprintIds, Action<SimBlueprint[]> onComplete)
    {
        List<Request> requests = new List<Request>(blueprintIds.Count);
        SimBlueprint[] results = new SimBlueprint[requests.Count];


        ////////////////////////////////////////////////////////////////////////////////////////
        //      Parse all blueprint ids and load their corresponding scenes
        ////////////////////////////////////////////////////////////////////////////////////////
        Dictionary<ISceneMetaData, AsyncOperation> sceneLoads = new Dictionary<ISceneMetaData, AsyncOperation>();
        LoadSceneParameters sceneLoadParameters = default;
        sceneLoadParameters.loadSceneMode = LoadSceneMode.Additive;
        sceneLoadParameters.localPhysicsMode = LocalPhysicsMode.None;

        for (int i = 0; i < blueprintIds.Count; i++)
        {
            if (ParseBlueprintId(blueprintIds[i], out string sceneGuid, out string gameObjectGuid) == false)
            {
                Debug.LogError($"Could not parse blueprintId {requests[i]} in SimBlueprintSceneObjectProvider");
                continue;
            }

            // find corresponding scene
            ISceneMetaData sceneMetaData = SceneMetaDataBank.SceneMetaDatas.Find((ISceneMetaData metaData) => metaData.AssetGuid == sceneGuid);

            if (sceneMetaData == null)
            {
                Debug.LogError($"Could not provide blueprint for {blueprintIds[i]} because no scene was found with the matching guid.");
                continue;
            }

            // start loading scene if it's not already
            if (sceneLoads.ContainsKey(sceneMetaData) == false)
            {
                //sceneLoads.Add(sceneMetaData, SceneService.LoadAsync(sceneMetaData.Name, LoadSceneMode.Additive)); // TODO
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////
        //      Wait for all scenes to load
        ////////////////////////////////////////////////////////////////////////////////////////

        bool sceneStillLoading = true;
        while (sceneStillLoading)
        {
            sceneStillLoading = false;
            foreach (KeyValuePair<ISceneMetaData, AsyncOperation> pair in sceneLoads)
            {
                if (!pair.Value.isDone)
                {
                    // pair.Value.
                    // sceneStillLoading = true; TODO
                    break;
                }
            }

            yield return null;
        }
    }

    private void OnSceneLoaded(Scene loadedScene)
    {


    }


    public void EndProvideBlueprint()
    {
        _blueprintRequests.Clear();
        _isInProvidingProcess = false;
    }


    //bool _registeredToSceneLoads = false;
    //void RegisterToSceneLoads()
    //{
    //    if(!_registeredToSceneLoads)
    //    {
    //        SceneManager.sceneLoaded += Event_OnSceneLoaded;
    //        _registeredToSceneLoads = true;
    //    }
    //}

    //void UnregisterFromSceneLoads()
    //{
    //    if (_registeredToSceneLoads)
    //    {
    //        SceneManager.sceneLoaded -= Event_OnSceneLoaded;
    //        _registeredToSceneLoads = false;
    //    }
    //}


    //void OnDestroy()
    //{
    //    UnregisterFromSceneLoads();
    //}



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

    public static SimBlueprintId MakeBlueprintId(string sceneGuid, string gameObjectGuid)
    {
        return new SimBlueprintId(SimBlueprintId.BlueprintType.SceneGameObject, $"{gameObjectGuid}/{sceneGuid}");
    }
    #endregion
}
