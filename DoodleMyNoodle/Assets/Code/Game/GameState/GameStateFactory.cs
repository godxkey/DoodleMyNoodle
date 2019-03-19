using System;

public class GameStateFactory
{
    public GameState CreateGameState(GameStateSettings settings)
    {
        return _factory.CreateValue(settings.GetType());
    }


    Factory<Type, GameState> _factory = new Factory<Type, GameState>();

    public GameStateFactory()
    {
        // default empty state
        //Register<Internals.GameStateManager.GameStateEmpty, GameStateSettings>();

        Register<GameStateInGameLocal, GameStateSettingsInGameLocal>();
        Register<GameStateInGameOnline, GameStateSettingsInGameOnline>();
        Register<GameStateLobbyClient, GameStateSettingsLobbyClient>();
        Register<GameStateLobbyLocal, GameStateSettingsLobbyLocal>();
        Register<GameStateLobbyServer, GameStateSettingsLobbyServer>();
        Register<GameStateRootMenu, GameStateSettingsRootMenu>();
    }

    void Register<LogicClass, SettingsClass>() where LogicClass : GameState, new()
    {
        _factory.Register<LogicClass>(typeof(SettingsClass));
    }

}
