using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MiniGameManager : GamePresentationSystem<MiniGameManager>
{
    private Action<GameAction.ParameterData> _currentTileSelectionCallback;

    protected override void OnGamePresentationUpdate()
    {
        if (Cache.LocalController != Entity.Null)
        {
            
        }
    }

    public void TriggerMiniGame(GameActionParameterMiniGame.Description parameterDescription, MiniGameDescriptionBase miniGameDescription,  Action<GameAction.ParameterData> onCompleteCallback)
    {
        _currentTileSelectionCallback = onCompleteCallback;


    }
}