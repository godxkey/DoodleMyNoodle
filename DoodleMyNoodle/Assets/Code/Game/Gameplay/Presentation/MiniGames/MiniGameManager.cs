using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class MiniGameManager : GamePresentationSystem<MiniGameManager>
{
    public Transform MiniGameContainer;

    private Action<GameAction.ParameterData> _currentMiniGameCallback;

    public void TriggerMiniGame(GameActionParameterMiniGame.Description parameterDescription, MiniGameDefinitionBase miniGameDefinition,  Action<GameAction.ParameterData> onCompleteCallback)
    {
        _currentMiniGameCallback = onCompleteCallback;

        Vector3 miniGameLocation = new Vector3(parameterDescription.MiniGameLocation.x, parameterDescription.MiniGameLocation.y, 0);
        GameObject newMiniGame = Instantiate(miniGameDefinition.Prefab, miniGameLocation, Quaternion.identity, MiniGameContainer);

        MiniGameBaseController miniGameController = newMiniGame.GetComponent<MiniGameBaseController>();

        if (miniGameController != null)
        {
            miniGameController.StartMiniGame(miniGameDefinition.CustomDescription, delegate(GameActionParameterMiniGame.Data resultData)
            {
                // can't start mini-game, complete it immediatly
                _currentMiniGameCallback.Invoke(resultData);
            });
        }
        else
        {
            // can't start mini-game, complete it immediatly
            GameActionParameterMiniGame.Data MiniGameResultData = new GameActionParameterMiniGame.Data();
            _currentMiniGameCallback.Invoke(MiniGameResultData);
        }
    }
}