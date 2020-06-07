using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManagerImpl : GameStateManager
{
    class Factory : GameStateFactory
    {
        public Factory()
        {
            Register<GameStateInGameClient, GameStateDefinitionInGameClient>();
            Register<GameStateInGameLocal, GameStateDefinitionInGameLocal>();
            Register<GameStateInGameServer, GameStateDefinitionInGameServer>();
            Register<GameStateLobbyClient, GameStateDefinitionLobbyClient>();
            Register<GameStateLobbyLocal, GameStateDefinitionLobbyLocal>();
            Register<GameStateLobbyServer, GameStateDefinitionLobbyServer>();
            Register<GameStateRootMenu, GameStateDefinitionRootMenu>();
        }
    }

    protected override GameStateFactory CreateFactory() => new Factory();
}
