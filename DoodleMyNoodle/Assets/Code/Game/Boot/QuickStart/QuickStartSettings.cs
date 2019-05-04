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

    public int localProfileId;
    public PlayMode playMode;
    public string serverName;
    public string level;

    public override string ToString()
    {
        return string.Format("Profile[{0}]  PlayMode[{1}]  ServerName[{2}]  Level[{3}]", localProfileId, playMode.ToString(), serverName, level);
    }
}