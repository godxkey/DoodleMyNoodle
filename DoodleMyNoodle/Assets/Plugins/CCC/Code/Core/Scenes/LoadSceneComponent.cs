using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class LoadSceneComponent : MonoBehaviour
{
    [SerializeField] SceneInfo sceneInfo;
    [SerializeField] bool loadOnStart = false;
    [SerializeField] bool dontLoadDuplicate = true;
    [SerializeField] bool loadAsync = false;

    public SceneInfo SceneInfo { get { return sceneInfo; } set { sceneInfo = value; } }

    [Serializable]
    public class SceneEvent : UnityEvent<Scene> { }
    public SceneEvent OnLoadComplete { get; set; }

    void Start()
    {
        if (loadOnStart)
        {
            CoreServiceManager.AddInitializationCallback(Load);
        }
    }

    public void Load()
    {
        Load(null);
    }
    public void Load(Action<Scene> callback)
    {
        if (dontLoadDuplicate && SceneService.IsLoadedOrBeingLoaded(sceneInfo))
            return;

        Action<Scene> localCallback = (scene) =>
        {
            callback?.Invoke(scene);
            OnLoadComplete?.Invoke(scene);
        };

        if (loadAsync)
            SceneService.LoadAsync(sceneInfo, localCallback);
        else
            SceneService.Load(sceneInfo, localCallback);
    }
}
