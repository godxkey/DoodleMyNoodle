using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateSettings : ScriptableObject
{
    [SerializeField] SceneInfo _sceneToLoadOnEnter;
    public SceneInfo sceneToLoadOnEnter => _sceneToLoadOnEnter;

    [HideIf("SceneToLoadOnEnterIsNull", HideShowBaseAttribute.Type.Method)]
    [SerializeField] SceneLoadSettings _sceneLoadSettings;
    public SceneLoadSettings sceneLoadSettings => _sceneLoadSettings;

    // Editor helper method
    bool SceneToLoadOnEnterIsNull() => sceneToLoadOnEnter == null;
}
