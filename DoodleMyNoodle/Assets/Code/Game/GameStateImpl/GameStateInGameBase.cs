﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateInGameBase<SettingsClass> : GameState<SettingsClass>, IGameStateInGameBase
    where SettingsClass : GameStateSettingsInGameBase
{

    public virtual void ReturnToMenu()
    {
        GameStateManager.TransitionToState(specificSettings.gameStateIfReturn);
    }

    public virtual void ExitApplication()
    {
        Application.Quit();
    }
}

public interface IGameStateInGameBase
{
    void ReturnToMenu();
    void ExitApplication();
}