using CCC.Online;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncedObjectsSystem : GameSystem<SyncedObjectsSystem>
{
    SyncedValues.Driver _driver;

    public override bool SystemReady => _driver != null && _driver.IsReady;

    public override void OnGameReady()
    {
        base.OnGameReady();

        switch (GameStateManager.currentGameState)
        {
            case GameStateInGameServer serverGameState:
                _driver = new SyncedValues.DriverServer(serverGameState.SessionInterface);
                break;

            case GameStateInGameLocal localGameState:
                _driver = new SyncedValues.DriverLocal();
                break;

            case GameStateInGameClient clientGameState:
                _driver = new SyncedValues.DriverClient(clientGameState.SessionInterface);
                break;
        }
    }

    protected override void OnDestroy()
    {
        _driver?.Dispose();

        base.OnDestroy();
    }
}
