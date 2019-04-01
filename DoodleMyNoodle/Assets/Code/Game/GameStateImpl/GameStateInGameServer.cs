using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateInGameServer : GameStateInGameOnline<GameStateDefinitionInGameServer>
{
    public override void Enter(GameStateParam[] parameters)
    {
        base.Enter(parameters);

        GameStateParamLevelName levelNameParam = parameters.Find<GameStateParamLevelName>();

        if (levelNameParam != null)
        {
            // do something?
            Debug.Log("level name: " + levelNameParam.levelName);
        }
    }
}
