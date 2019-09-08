using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public abstract class GameState
{
    public GameStateDefinition Definition { get; private set; }
    
    public virtual void SetDefinition(GameStateDefinition definition)
    {
        this.Definition = definition;
    }

    public virtual void Enter(GameStateParam[] parameters)
    {
        if (Definition.sceneToLoadOnEnter != null && !SceneService.IsLoadedOrBeingLoaded(Definition.sceneToLoadOnEnter))
        {
            SceneService.Load(Definition.sceneToLoadOnEnter, Definition.sceneLoadSettings, OnDefaultSceneLoaded);
        }
    }

    protected virtual void OnDefaultSceneLoaded(Scene scene)
    {

    }

    public virtual void Update()
    {

    }

    public virtual void BeginExit(GameStateParam[] parameters)
    {

    }

#if DEBUG_BUILD
    public virtual void OnDebugPanelGUI()
    {

    }
#endif

    public virtual bool IsExitComplete()
    {
        return true;
    }
}