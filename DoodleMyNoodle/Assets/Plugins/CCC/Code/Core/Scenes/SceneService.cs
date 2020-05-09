using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public interface ISceneLoadPromise
{
    bool IsComplete { get; }
    Scene Scene { get; }
    event Action<ISceneLoadPromise> OnComplete;
    string SceneName { get; }
}

public class SceneService : MonoCoreService<SceneService>
{
    private class ScenePromise : ISceneLoadPromise
    {
        public ScenePromise(string sceneName)
        {
            SceneName = sceneName;
            IsComplete = false;
        }

        public string SceneName { get; set; }
        private event Action<ISceneLoadPromise> OnCompleteInternal;
        public event Action<ISceneLoadPromise> OnComplete
        {
            add
            {
                if (IsComplete)
                    value(this);
                else
                    OnCompleteInternal += value;
            }
            remove
            {
                OnCompleteInternal -= value;
            }
        }
        public Scene Scene { get; set; }
        public bool IsComplete { get; private set; }

        public void Complete()
        {
            IsComplete = true;
            OnCompleteInternal?.SafeInvoke(this);
        }
    }

    List<ScenePromise> _loadingScenePromises = new List<ScenePromise>();
    List<ScenePromise> _unloadingScenePromises = new List<ScenePromise>();

    // useful to know if we're the first scene in the game
    public static int TotalSceneLoadCount { get; private set; }

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

    public static ISceneLoadPromise Load(string sceneName, SceneLoadSettings loadSettings)
    {
        // add new promise
        ScenePromise scenePromise = new ScenePromise(sceneName);
        Instance._loadingScenePromises.Add(scenePromise);


        LoadSceneParameters loadParameters = new LoadSceneParameters()
        {
            loadSceneMode = loadSettings.LoadSceneMode,
            localPhysicsMode = loadSettings.LocalPhysicsMode
        };

        try
        {
            switch (loadSettings.LoadSceneMode)
            {
                case LoadSceneMode.Single:
                    SceneManager.LoadScene(sceneName, loadParameters);
                    break;
                case LoadSceneMode.Additive:
                    SceneManager.LoadSceneAsync(sceneName, loadParameters);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        return scenePromise;
    }
    public static ISceneLoadPromise Load(SceneInfo sceneInfo, SceneLoadSettings loadSettings)
    {
        return Load(sceneInfo.SceneName, loadSettings);
    }

    public static ISceneLoadPromise Load(string sceneName, LoadSceneMode mode = LoadSceneMode.Additive, LocalPhysicsMode localPhysicsMode = LocalPhysicsMode.Physics3D)
    {
        return Load(sceneName, new SceneLoadSettings() { Async = false, LoadSceneMode = mode, LocalPhysicsMode = localPhysicsMode });
    }
    // fbessette: should not be used anymore. If we add it back, we should refactor it a bit so that SceneInfo doesn't contain all of these misplaced
    //            properties like the 'default load scene mode'
    //public static void Load(SceneInfo sceneInfo, Action<Scene> callback = null)
    //{
    //    Load(sceneInfo.SceneName, sceneInfo.LoadMode, callback, sceneInfo.AllowMultiple);
    //}

    public static ISceneLoadPromise LoadAsync(string sceneName, LoadSceneMode mode = LoadSceneMode.Additive, LocalPhysicsMode localPhysicsMode = LocalPhysicsMode.Physics3D)
    {
        return Load(sceneName, new SceneLoadSettings() { Async = true, LoadSceneMode = mode, LocalPhysicsMode = localPhysicsMode });
    }
    // fbessette: should not be used anymore. If we add it back, we should refactor it a bit so that SceneInfo doesn't contain all of these misplaced
    //            properties like the 'default load scene mode'
    //public static void LoadAsync(SceneInfo sceneInfo, Action<Scene> callback = null)
    //{
    //    LoadAsync(sceneInfo.SceneName, sceneInfo.LoadMode, callback, sceneInfo.AllowMultiple);
    //}

    //private static bool TryHandleUniqueLoad(string sceneName, Action<Scene> callback)
    //{
    //    ScenePromise scenePromise = GetLoadingScenePromise(sceneName);
    //    if (scenePromise != null) // means the scene is currently being loaded
    //    {
    //        scenePromise.OnComplete += callback;
    //        return true;
    //    }
    //    else if (IsLoaded(sceneName))
    //    {
    //        callback?.Invoke(GetLoaded(sceneName));
    //        return true;
    //    }
    //    return false;
    //}

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
        return IsLoaded(sceneName) || IsLoading(sceneName);
    }
    public static bool IsLoadedOrBeingLoaded(SceneInfo sceneInfo)
    {
        return IsLoadedOrLoading(sceneInfo.SceneName);
    }

    public static ISceneLoadPromise FindLoadingScenePromise(string sceneName)
    {
        for (int i = 0; i < Instance._loadingScenePromises.Count; i++)
        {
            if (Instance._loadingScenePromises[i].SceneName == sceneName)
                return Instance._loadingScenePromises[i];
        }
        return null;
    }

    public static bool IsLoading(string sceneName)
    {
        return FindLoadingScenePromise(sceneName) != null;
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

    public static int LoadedSceneCount
    {
        get { return SceneManager.sceneCount; }
    }
    public static int LoadingSceneCount
    {
        get { return Instance._loadingScenePromises.Count; }
    }

    #endregion

    #region InLoading Events

    static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TotalSceneLoadCount++;
        ScenePromise promise = GetLoadingScenePromise(scene.name);
        if (promise == null)
            return;

        promise.Scene = scene;

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
        promise.Complete();

        Instance._loadingScenePromises.Remove(promise);
    }

    #endregion

    #region Internal Utility

    static ScenePromise GetLoadingScenePromise(string name)
    {
        foreach (ScenePromise scene in Instance._loadingScenePromises)
        {
            if (scene.SceneName == name) return scene;
        }
        return null;
    }
    static ScenePromise GetUnloadingScenePromise(string name)
    {
        foreach (ScenePromise scene in Instance._unloadingScenePromises)
        {
            if (scene.SceneName == name) return scene;
        }
        return null;
    }

    #endregion
}