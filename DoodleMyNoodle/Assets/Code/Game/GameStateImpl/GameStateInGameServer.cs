using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateInGameServer : GameStateInGameOnline
{
    public string levelToPlay { get; private set; }

    public override void Enter(GameStateParam[] parameters)
    {
        base.Enter(parameters);

        GameStateParamLevelName levelNameParam = parameters.Find<GameStateParamLevelName>();

        if (levelNameParam != null)
        {
            // do something?
            levelToPlay = levelNameParam.levelName;
        }
    }
}
