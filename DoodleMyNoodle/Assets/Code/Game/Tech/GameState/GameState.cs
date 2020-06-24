using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public abstract class GameState
{
    public GameStateDefinition Definition { get; private set; }

    ISceneLoadPromise _sceneLoadPromise;

    public virtual void SetDefinition(GameStateDefinition definition)
    {
        this.Definition = definition;
    }

    public virtual void Enter(GameStateParam[] parameters)
    {
        if (Definition.SceneToLoadOnEnter != null && !SceneService.IsLoadedOrBeingLoaded(Definition.SceneToLoadOnEnter))
        {
            _sceneLoadPromise = SceneService.Load(Definition.SceneToLoadOnEnter, Definition.SceneLoadSettings);
            _sceneLoadPromise.OnComplete += OnDefaultSceneLoaded;
        }
    }

    protected virtual void OnDefaultSceneLoaded(ISceneLoadPromise sceneLoadPromise)
    {
        _sceneLoadPromise = null;
    }

    public virtual void Update()
    {

    }

    public virtual void BeginExit(GameStateParam[] parameters)
    {
        if (_sceneLoadPromise != null)
            _sceneLoadPromise.OnComplete -= OnDefaultSceneLoaded;
    }

#if DEBUG
    public virtual void OnDebugPanelGUI()
    {

    }
#endif

    public virtual bool IsExitComplete()
    {
        return true;
    }
}