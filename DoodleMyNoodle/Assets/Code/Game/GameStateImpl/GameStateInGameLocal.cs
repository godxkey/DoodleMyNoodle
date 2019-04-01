using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateInGameLocal : GameStateInGameBase<GameStateDefinitionInGameLocal>
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
