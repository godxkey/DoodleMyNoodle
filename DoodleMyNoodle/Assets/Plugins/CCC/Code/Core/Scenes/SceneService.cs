using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneService : MonoCoreService<SceneService>
{
    private class ScenePromise
    {
        public ScenePromise(string name, Action<Scene> callback)
        {
            this.name = name;
            Callbacks += callback;
        }
        public string name;
        public event Action<Scene> Callbacks;
        public Scene scene;
        public void InvokeCallback()
        {
            Callbacks?.SafeInvoke(scene);
        }
    }

    List<ScenePromise> loadingScenePromises = new List<ScenePromise>();
    List<ScenePromise> unloadingScenePromises = new List<ScenePromise>();

    public override void Initialize(Action onComplete)
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        //SceneManager.sceneUnloaded += OnSceneUnloaded;
        onComplete();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        //SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    #region Load/Unload Methods

    public void Load(string name, LoadSceneMode mode = LoadSceneMode.Single, Action<Scene> callback = null, bool allowMultiple = true)
    {
        if (!allowMultiple && _HandleUniqueLoad(name, callback))
            return;

        ScenePromise scenePromise = new ScenePromise(name, callback);
        loadingScenePromises.Add(scenePromise);
        SceneManager.LoadScene(name, mode);
    }
    public void Load(SceneInfo sceneInfo, Action<Scene> callback = null)
    {
        Load(sceneInfo.SceneName, sceneInfo.LoadMode, callback, sceneInfo.AllowMultiple);
    }

    public void LoadAsync(string name, LoadSceneMode mode = LoadSceneMode.Single, Action<Scene> callback = null, bool allowMultiple = true)
    {
        if (!allowMultiple && _HandleUniqueLoad(name, callback))
            return;

        ScenePromise scenePromise = new ScenePromise(name, callback);
        loadingScenePromises.Add(scenePromise);
        SceneManager.LoadSceneAsync(name, mode);
    }
    public void LoadAsync(SceneInfo sceneInfo, Action<Scene> callback = null)
    {
        LoadAsync(sceneInfo.SceneName, sceneInfo.LoadMode, callback, sceneInfo.AllowMultiple);
    }

    private bool _HandleUniqueLoad(string sceneName, Action<Scene> callback)
    {
        ScenePromise scenePromise = GetLoadingScenePromise(sceneName);
        if (scenePromise != null) // means the scene is currently being loaded
        {
            scenePromise.Callbacks += callback;
            return true;
        }
        else if (IsActive(sceneName))
        {
            callback?.Invoke(GetActive(sceneName));
            return true;
        }
        return false;
    }

    public void UnloadAsync(Scene scene)
    {
        SceneManager.UnloadSceneAsync(scene);
    }
    public void UnloadAsync(string name)
    {
        SceneManager.UnloadSceneAsync(name);
    }
    public void UnloadAsync(SceneInfo sceneInfo)
    {
        UnloadAsync(sceneInfo.SceneName);
    }

    public bool IsActiveOrBeingLoaded(string sceneName)
    {
        if (IsActive(sceneName) || IsBeingLoaded(sceneName))
            return true;
        return false;
    }
    public bool IsActiveOrBeingLoaded(SceneInfo sceneInfo)
    {
        return IsActiveOrBeingLoaded(sceneInfo.SceneName);
    }

    public bool IsBeingLoaded(string sceneName)
    {
        for (int i = 0; i < loadingScenePromises.Count; i++)
        {
            if (loadingScenePromises[i].name == sceneName) return true;
        }
        return false;
    }
    public bool IsBeingLoaded(SceneInfo sceneInfo)
    {
        return IsBeingLoaded(sceneInfo.SceneName);
    }

    public bool IsActive(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == sceneName) return true;
        }
        return false;
    }
    public bool IsActive(SceneInfo sceneInfo)
    {
        return IsActive(sceneInfo.SceneName);
    }

    public Scene GetActive(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == sceneName) return SceneManager.GetSceneAt(i);
        }
        throw new Exception("No active scene by that name: " + sceneName);
    }
    public Scene GetActive(SceneInfo sceneInfo)
    {
        return GetActive(sceneInfo.SceneName);
    }

    public int ActiveSceneCount
    {
        get { return SceneManager.sceneCount; }
    }
    public int LoadingSceneCount
    {
        get { return loadingScenePromises.Count; }
    }

    #endregion

    #region InLoading Events

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ScenePromise promise = GetLoadingScenePromise(scene.name);
        if (promise == null)
            return;

        promise.scene = scene;

        if (!scene.isLoaded)
            StartCoroutine(WaitForSceneLoad(scene, promise));
        else
            Execute(promise);
    }
    void OnSceneUnloaded(Scene scene)
    {
        ScenePromise promise = GetUnloadingScenePromise(scene.name);
        if (promise == null)
            return;

        promise.scene = scene;

        if (scene.isLoaded)
            StartCoroutine(WaitForSceneUnload(scene, promise));
        else
            Execute(promise);
    }

    IEnumerator WaitForSceneLoad(Scene scene, ScenePromise promise)
    {
        while (!scene.isLoaded)
            yield return null;
        Execute(promise);
    }

    IEnumerator WaitForSceneUnload(Scene scene, ScenePromise promise)
    {
        while (scene.isLoaded)
            yield return null;
        Execute(promise);
    }

    void Execute(ScenePromise promise)
    {
        promise.InvokeCallback();

        loadingScenePromises.Remove(promise);
    }

    #endregion

    #region Internal Utility

    ScenePromise GetLoadingScenePromise(string name)
    {
        foreach (ScenePromise scene in loadingScenePromises)
        {
            if (scene.name == name) return scene;
        }
        return null;
    }
    ScenePromise GetUnloadingScenePromise(string name)
    {
        foreach (ScenePromise scene in unloadingScenePromises)
        {
            if (scene.name == name) return scene;
        }
        return null;
    }

    #endregion
}