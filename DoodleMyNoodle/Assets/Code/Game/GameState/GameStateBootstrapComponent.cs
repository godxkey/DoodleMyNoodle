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
        if(GameStateManager.Instance.targetGameState == null && gameStateSettings != null)
        {
            GameStateManager.Instance.TransitionToState(gameStateSettings);
        }
    }
}
