using System;


[Serializable]
public class GameStateParamLevelName : GameStateParam
{
    public GameStateParamLevelName(string levelName)
    {
        this.levelName = levelName;
    }

    public readonly string levelName;
}