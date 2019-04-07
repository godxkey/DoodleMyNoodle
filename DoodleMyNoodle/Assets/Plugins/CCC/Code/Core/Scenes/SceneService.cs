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

    List<ScenePromise> _loadingScenePromises = new List<ScenePromise>();
    List<ScenePromise> _unloadingScenePromises = new List<ScenePromise>();

    // useful to know if we're the first scene in the game
    public static int totalSceneLoadCount { get; private set; } 

    public override void Initialize(Action<ICoreService> onComplete)
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        //SceneManager.sceneUnloaded += OnSceneUnloaded;
        onComplete(this);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        //SceneManager.sceneUnloaded -= OnSceneUnloaded;
    }

    #region Load/Unload Methods

    public static void Load(SceneInfo sceneInfo, SceneLoadSettings loadSettings, Action<Scene> callback = null)
    {
        if (loadSettings.async)
        {
            LoadAsync(sceneInfo.SceneName, loadSettings.additive ? LoadSceneMode.Additive : LoadSceneMode.Single, callback, loadSettings.allowMultiple);
        }
        else
        {
            Load(sceneInfo.SceneName, loadSettings.additive ? LoadSceneMode.Additive : LoadSceneMode.Single, callback, loadSettings.allowMultiple);
        }
    }
    public static void Load(string name, LoadSceneMode mode = LoadSceneMode.Single, Action<Scene> callback = null, bool allowMultiple = true)
    {
        if (!allowMultiple && TryHandleUniqueLoad(name, callback))
            return;

        ScenePromise scenePromise = new ScenePromise(name, callback);
        Instance._loadingScenePromises.Add(scenePromise);
        SceneManager.LoadScene(name, mode);
    }
    public static void Load(SceneInfo sceneInfo, Action<Scene> callback = null)
    {
        Load(sceneInfo.SceneName, sceneInfo.LoadMode, callback, sceneInfo.AllowMultiple);
    }

    public static void LoadAsync(string name, LoadSceneMode mode = LoadSceneMode.Single, Action<Scene> callback = null, bool allowMultiple = true)
    {
        if (!allowMultiple && TryHandleUniqueLoad(name, callback))
            return;

        ScenePromise scenePromise = new ScenePromise(name, callback);
        Instance._loadingScenePromises.Add(scenePromise);
        SceneManager.LoadSceneAsync(name, mode);
    }
    public static void LoadAsync(SceneInfo sceneInfo, Action<Scene> callback = null)
    {
        LoadAsync(sceneInfo.SceneName, sceneInfo.LoadMode, callback, sceneInfo.AllowMultiple);
    }

    private static bool TryHandleUniqueLoad(string sceneName, Action<Scene> callback)
    {
        ScenePromise scenePromise = GetLoadingScenePromise(sceneName);
        if (scenePromise != null) // means the scene is currently being loaded
        {
            scenePromise.Callbacks += callback;
            return true;
        }
        else if (IsLoaded(sceneName))
        {
            callback?.Invoke(GetLoaded(sceneName));
            return true;
        }
        return false;
    }

    public static void UnloadAsync(Scene scene)
    {
        SceneManager.UnloadSceneAsync(scene);
    }
    public static void UnloadAsync(string name)
    {
        SceneManager.UnloadSceneAsync(name);
    }
    public static void UnloadAsync(SceneInfo sceneInfo)
    {
        UnloadAsync(sceneInfo.SceneName);
    }

    public static bool IsLoadedOrLoading(string sceneName)
    {
        if (IsLoaded(sceneName) || IsLoading(sceneName))
            return true;
        return false;
    }
    public static bool IsLoadedOrBeingLoaded(SceneInfo sceneInfo)
    {
        return IsLoadedOrLoading(sceneInfo.SceneName);
    }

    public static bool IsLoading(string sceneName)
    {
        for (int i = 0; i < Instance._loadingScenePromises.Count; i++)
        {
            if (Instance._loadingScenePromises[i].name == sceneName) return true;
        }
        return false;
    }
    public static bool IsLoading(SceneInfo sceneInfo)
    {
        return IsLoading(sceneInfo.SceneName);
    }

    public static bool IsLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == sceneName) return true;
        }
        return false;
    }
    public static bool IsLoaded(SceneInfo sceneInfo)
    {
        return IsLoaded(sceneInfo.SceneName);
    }

    public static Scene GetLoaded(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            if (SceneManager.GetSceneAt(i).name == sceneName) return SceneManager.GetSceneAt(i);
        }
        throw new Exception("No active scene by that name: " + sceneName);
    }
    public static Scene GetLoaded(SceneInfo sceneInfo)
    {
        return GetLoaded(sceneInfo.SceneName);
    }

    public static Scene GetActiveScene() => SceneManager.GetActiveScene();
    public static void SetActiveScene(Scene scene) => SceneManager.SetActiveScene(scene);

    public static int loadedSceneCount
    {
        get { return SceneManager.sceneCount; }
    }
    public static int loadingSceneCount
    {
        get { return Instance._loadingScenePromises.Count; }
    }

    #endregion

    #region InLoading Events

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        totalSceneLoadCount++;
        ScenePromise promise = GetLoadingScenePromise(scene.name);
        if (promise == null)
            return;

        promise.scene = scene;

        if (!scene.isLoaded)
            Instance.StartCoroutine(WaitForSceneLoad(scene, promise));
        else
            Execute(promise);
    }
    //void OnSceneUnloaded(Scene scene)
    //{
    //    ScenePromise promise = GetUnloadingScenePromise(scene.name);
    //    if (promise == null)
    //        return;

    //    promise.scene = scene;

    //    if (scene.isLoaded)
    //        StartCoroutine(WaitForSceneUnload(scene, promise));
    //    else
    //        Execute(promise);
    //}

    static IEnumerator WaitForSceneLoad(Scene scene, ScenePromise promise)
    {
        while (!scene.isLoaded)
            yield return null;
        Execute(promise);
    }

    //IEnumerator WaitForSceneUnload(Scene scene, ScenePromise promise)
    //{
    //    while (scene.isLoaded)
    //        yield return null;
    //    Execute(promise);
    //}

    static void Execute(ScenePromise promise)
    {
        promise.InvokeCallback();

        Instance._loadingScenePromises.Remove(promise);
    }

    #endregion

    #region Internal Utility

    static ScenePromise GetLoadingScenePromise(string name)
    {
        foreach (ScenePromise scene in Instance._loadingScenePromises)
        {
            if (scene.name == name) return scene;
        }
        return null;
    }
    static ScenePromise GetUnloadingScenePromise(string name)
    {
        foreach (ScenePromise scene in Instance._unloadingScenePromises)
        {
            if (scene.name == name) return scene;
        }
        return null;
    }

    #endregion
}