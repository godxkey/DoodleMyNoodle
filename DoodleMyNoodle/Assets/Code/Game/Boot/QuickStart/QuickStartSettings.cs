using System;


[Serializable]
public struct QuickStartSettings
{
    public enum PlayMode
    {
        // do not change the values
        Local = 0,
        OnlineClient = 1,
        OnlineServer = 2,
    }

    public PlayMode playMode;
    public string playerName;
    public string serverName;
    public string level;

    public override string ToString()
    {
        return string.Format("PlayMode[{0}]  PlayerName[{1}]  ServerName[{2}]  Level[{3}]", playMode.ToString(), playerName, serverName, level);
    }
}