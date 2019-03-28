using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateBootstrapComponent : MonoBehaviour
{
    public GameStateSettings gameStateSettings;

    void Awake()
    {
        CoreServiceManager.AddInitializationCallback<GameStateManager>(OnGameStateManagerInit);
    }

    void OnGameStateManagerInit()
    {
        if(GameStateManager.targetGameState == null && gameStateSettings != null)
        {
            GameStateManager.TransitionToState(gameStateSettings);
        }
    }
}
