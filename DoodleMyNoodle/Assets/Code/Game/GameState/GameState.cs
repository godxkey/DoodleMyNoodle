using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public abstract class GameState
{
    public GameStateManager gameStateManager { get; set; }
    public GameStateSettings settings { get; private set; }
    
    public virtual void SetSettings(GameStateSettings settings)
    {
        this.settings = settings;
    }

    public virtual void Enter()
    {
        if(settings.sceneToLoadOnEnter != null)
        {
            SceneService.Load(settings.sceneToLoadOnEnter, settings.sceneLoadSettings, OnDefaultSceneLoaded);
        }
    }

    protected virtual void OnDefaultSceneLoaded(Scene scene)
    {

    }

    public virtual void Update()
    {

    }

    public virtual void BeginExit()
    {

    }

    public virtual bool IsExitComplete()
    {
        return true;
    }
}