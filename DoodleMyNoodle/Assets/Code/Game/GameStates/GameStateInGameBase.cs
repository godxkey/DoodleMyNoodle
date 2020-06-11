using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateInGameBase : GameState
{
    public virtual void ReturnToMenu()
    {
        GameStateManager.TransitionToState(((GameStateDefinitionInGameBase)Definition).gameStateIfReturn);
    }

    public override void Update()
    {
        base.Update();

        // Hackish solution - TO REMOVE
        if (SimEndGameManager.ReturnToMenu)
        {
            SimEndGameManager.ReturnToMenu = false;
            ReturnToMenu();
        }
    }

    public virtual void ExitApplication()
    {
        Application.Quit();
    }
}