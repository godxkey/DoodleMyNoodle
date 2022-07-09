using System;


[Serializable]
public class GameStateParamMapName : GameStateParam
{
    public GameStateParamMapName(string mapName)
    {
        this.MapName = mapName;
    }

    public readonly string MapName;
}