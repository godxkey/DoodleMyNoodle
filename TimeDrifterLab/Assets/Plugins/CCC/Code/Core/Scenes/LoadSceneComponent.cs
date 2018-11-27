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
        if (dontLoadDuplicate && SceneService.Instance.IsActiveOrBeingLoaded(sceneInfo))
            return;

        Action<Scene> localCallback = (scene) =>
        {
            callback?.Invoke(scene);
            OnLoadComplete?.Invoke(scene);
        };

        if (loadAsync)
            SceneService.Instance.LoadAsync(sceneInfo, localCallback);
        else
            SceneService.Instance.Load(sceneInfo, localCallback);
    }
}
