using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

public class GameActionRequestManager : GamePresentationSystem<GameActionRequestManager>
{
    [SerializeField] private Transform _requestContainer;

    private Action<List<GameAction.ParameterData>> _currentRequestCallback;

    public void RequestData(Vector3 requestLocation, GameActionRequestDefinitionBase gameActionRequestDefinition,  Action<List<GameAction.ParameterData>> onCompleteCallback, params GameAction.ParameterDescription[] parameters)
    {
        _currentRequestCallback = onCompleteCallback;

        // No Request Filled, we fall back to defautl cases where we request with external systems and managers (tile highlight)
        if (gameActionRequestDefinition == null)
        {
            RequestExternalData(parameters);
            return;
        }

        GameObject gameActionDataRequestInstance = Instantiate(gameActionRequestDefinition.gameObject, requestLocation, Quaternion.identity, _requestContainer);

        GameActionDataRequestBaseController requestController = gameActionDataRequestInstance.GetComponent<GameActionDataRequestBaseController>();

        if (requestController != null)
        {
            requestController.StartRequest(gameActionRequestDefinition, delegate(List<GameAction.ParameterData> resultData)
            {
                _currentRequestCallback.Invoke(resultData);
            });
        }
        else
        {
            Debug.LogError("Skipping request because it couldn't start");

            // can't start mini-game, complete it immediatly
            List<GameAction.ParameterData> DefaultResults = new List<GameAction.ParameterData>();
            _currentRequestCallback.Invoke(DefaultResults);
        }
    }

    public void RequestExternalData(GameAction.ParameterDescription[] parameters)
    {
        List<GameAction.ParameterData> Results = new List<GameAction.ParameterData>();

        int parameterRequested = 0;
        for (int i = 0; i < parameters.Length; i++)
        {
            GameAction.ParameterDescription currentParameter = parameters[i];

            if (currentParameter != null)
            {
                if (currentParameter is GameActionParameterTile.Description TileDescription)
                {
                    if (TileDescription != null)
                    {
                        parameterRequested++;
                        TileHighlightManager.Instance.AskForSingleTileSelectionAroundPlayer(TileDescription, (GameAction.ParameterData TileSelectedData) =>
                        {
                            Results.Add(TileSelectedData);

                            // Support multiple parameters, each callback we verify if we're ready to complete
                            if (Results.Count >= parameterRequested)
                            {
                                _currentRequestCallback.Invoke(Results);
                            }
                        });
                    }
                }
            }
        }
    }
}