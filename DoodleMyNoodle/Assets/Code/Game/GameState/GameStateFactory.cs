using System;

public abstract class GameStateFactory
{
    public GameState CreateGameState(GameStateDefinition settings)
    {
        return _factory.CreateValue(settings.GetType());
    }


    Factory<Type, GameState> _factory = new Factory<Type, GameState>();

    protected void Register<LogicClass, SettingsClass>() where LogicClass : GameState, new()
    {
        _factory.Register<LogicClass>(typeof(SettingsClass));
    }

}
