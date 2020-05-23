using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

public class GameStateInGameServer : GameStateInGameOnline
{
    public string LevelToPlay { get; private set; }

    public override void Enter(GameStateParam[] parameters)
    {
        base.Enter(parameters);

        GameStateParamLevelName levelNameParam = parameters.Find<GameStateParamLevelName>();

        if (levelNameParam != null)
        {
            // do something?
            LevelToPlay = levelNameParam.levelName;
        }
    }
}
