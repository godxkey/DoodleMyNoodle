using System;

public class GameStateFactory
{
    public GameState CreateGameState(GameStateDefinition settings)
    {
        return _factory.CreateValue(settings.GetType());
    }


    Factory<Type, GameState> _factory = new Factory<Type, GameState>();

    public GameStateFactory()
    {
        // default empty state
        //Register<Internals.GameStateManager.GameStateEmpty, GameStateSettings>();

        Register<GameStateInGameClient, GameStateDefinitionInGameClient>();
        Register<GameStateInGameLocal, GameStateDefinitionInGameLocal>();
        Register<GameStateInGameServer, GameStateDefinitionInGameServer>();
        Register<GameStateLobbyClient, GameStateDefinitionLobbyClient>();
        Register<GameStateLobbyLocal, GameStateDefinitionLobbyLocal>();
        Register<GameStateLobbyServer, GameStateDefinitionLobbyServer>();
        Register<GameStateRootMenu, GameStateDefinitionRootMenu>();
    }

    void Register<LogicClass, SettingsClass>() where LogicClass : GameState, new()
    {
        _factory.Register<LogicClass>(typeof(SettingsClass));
    }

}
