using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateInGameLocal : GameStateInGameBase
{
    public string levelToPlay { get; private set; }

    public override void Enter(GameStateParam[] parameters)
    {
        base.Enter(parameters);


        GameStateParamLevelName levelNameParam = parameters.Find<GameStateParamLevelName>();

        if (levelNameParam != null)
        {
            levelToPlay = levelNameParam.levelName;
        }
    }
}
