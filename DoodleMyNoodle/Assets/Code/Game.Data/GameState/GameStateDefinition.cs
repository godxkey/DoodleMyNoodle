using System;
using UnityEngine;

public abstract class GameStateDefinition : ScriptableObject
{
    [SerializeField] SceneInfo _sceneToLoadOnEnter;
    public SceneInfo sceneToLoadOnEnter => _sceneToLoadOnEnter;

    [HideIf("SceneToLoadOnEnterIsNull", HideShowBaseAttribute.Type.Method)]
    [SerializeField] SceneLoadSettings _sceneLoadSettings;
    public SceneLoadSettings sceneLoadSettings => _sceneLoadSettings;

    // Editor helper method
    bool SceneToLoadOnEnterIsNull() => sceneToLoadOnEnter == null;


    public virtual void DeclareLinks(Action<GameStateDefinition> declare) { }
}
