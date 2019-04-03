using System;


[Serializable]
public struct QuickStartSettings
{
    public enum PlayMode
    {
        // do not change the values
        None = 0,
        Local = 1,
        OnlineClient = 2,
        OnlineServer = 3,
    }

    public PlayMode playMode;
    public string playerName;
    public string serverName;
    public string level;

    public override string ToString()
    {
        return string.Format("PlayMode[%s]  PlayerName[%s]  ServerName[%s]  Level[%s]", playMode.ToString(), playerName, serverName, level);
    }
}