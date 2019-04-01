using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public abstract class GameState
{
    public GameStateDefinition definition { get; private set; }
    
    public virtual void SetDefinition(GameStateDefinition definition)
    {
        this.definition = definition;
    }

    public virtual void Enter(GameStateParam[] parameters)
    {
        if (definition.sceneToLoadOnEnter != null && !SceneService.IsLoadedOrBeingLoaded(definition.sceneToLoadOnEnter))
        {
            SceneService.Load(definition.sceneToLoadOnEnter, definition.sceneLoadSettings, OnDefaultSceneLoaded);
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

public abstract class GameState<SettingsClass> : GameState where SettingsClass : GameStateDefinition
{
    public SettingsClass specificDefinition { get; private set; }

    public override void SetDefinition(GameStateDefinition settings)
    {
        base.SetDefinition(settings);
        specificDefinition = (SettingsClass)settings;
    }
}