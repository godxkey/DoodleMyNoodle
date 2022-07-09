using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

public class GameStateInGameServer : GameStateInGameOnline
{
    public string MapToPlay { get; private set; }

    public override void Enter(GameStateParam[] parameters)
    {
        base.Enter(parameters);

        GameStateParamMapName mapNameParam = parameters.Find<GameStateParamMapName>();

        if (mapNameParam != null)
        {
            // do something?
            MapToPlay = mapNameParam.MapName;
        }
    }
}
