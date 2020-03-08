using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

internal class SceneLoadedOnceService : MonoCoreService<SceneLoadedOnceService>
{
    const string SCENE_NAME = "LoadedOnceOnStart";

    public override void Initialize(Action<ICoreService> onComplete)
    {
        SceneService.LoadAsync(SCENE_NAME, LoadSceneMode.Additive)
            .OnComplete += (ISceneLoadPromise sceneLoadPromise) =>
        {
            foreach (GameObject go in sceneLoadPromise.Scene.GetRootGameObjects())
            {
                DontDestroyOnLoad(go);
            }
            SceneService.UnloadAsync(SCENE_NAME);
            onComplete(this);
        };
    }
}
