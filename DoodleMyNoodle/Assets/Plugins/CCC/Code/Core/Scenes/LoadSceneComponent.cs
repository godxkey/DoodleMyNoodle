using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LoadSceneComponent : MonoBehaviour
{
    [Serializable]
    public class SceneEvent : UnityEvent<Scene> { }

    [SerializeField] SceneInfo sceneInfo;
    [SerializeField] bool loadOnStart = false;
    [SerializeField] bool dontLoadDuplicate = true;
    [SerializeField] bool loadAsync = false;
    [SerializeField] SceneEvent onLoadComplete = new SceneEvent();

    public SceneInfo SceneInfo => sceneInfo;
    public SceneEvent OnLoadComplete => onLoadComplete;

    ISceneLoadPromise _loadPromise;

    void Start()
    {
        if (loadOnStart)
        {
            CoreServiceManager.AddInitializationCallback(Load);
        }
    }

    public void Load()
    {
        if (dontLoadDuplicate && SceneService.IsLoadedOrBeingLoaded(sceneInfo))
            return;


        _loadPromise = SceneService.Load(sceneInfo.SceneName, new SceneLoadSettings()
        {
            Async = loadAsync,
            LoadSceneMode = LoadSceneMode.Additive,
            LocalPhysicsMode = LocalPhysicsMode.Physics3D
        });

        _loadPromise.OnComplete += OnSceneLoaded;
    }

    void OnSceneLoaded(ISceneLoadPromise sceneLoadPromise)
    {
        onLoadComplete?.Invoke(sceneLoadPromise.Scene);
    }
}
