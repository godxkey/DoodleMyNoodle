using CCC.InspectorDisplay;
using System;
using UnityEngine;

public abstract class GameStateDefinition : ScriptableObject
{
    [SerializeField] SceneInfo _sceneToLoadOnEnter;
    public SceneInfo SceneToLoadOnEnter => _sceneToLoadOnEnter;

    [HideIf("SceneToLoadOnEnterIsNull")]
    [SerializeField] SceneLoadSettings _sceneLoadSettings;
    public SceneLoadSettings SceneLoadSettings => _sceneLoadSettings;

    // Editor helper method
    bool SceneToLoadOnEnterIsNull() => SceneToLoadOnEnter == null;


    public virtual void DeclareLinks(System.Action<GameStateDefinition> declare) { }
}
