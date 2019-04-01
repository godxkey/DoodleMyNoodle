using System;


[Serializable]
public struct QuickStartSettings
{
    public enum PlayMode
    {
        Local,
        OnlineClient,
        OnlineServer
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