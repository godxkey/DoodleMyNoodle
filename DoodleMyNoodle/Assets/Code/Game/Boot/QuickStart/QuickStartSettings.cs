using System;
using UnityEngine.Serialization;

[Serializable]
public struct QuickStartSettings
{
    public enum EPlayMode
    {
        // do not change the values
        Local = 0,
        OnlineClient = 1,
        OnlineServer = 2,
    }

    [FormerlySerializedAs("localProfileId")]
    public int LocalProfileId;

    [FormerlySerializedAs("playMode")]
    public EPlayMode PlayMode;

    [FormerlySerializedAs("serverName")]
    public string ServerName;

    [FormerlySerializedAs("level")]
    public string Map;

    public override string ToString()
    {
        return string.Format("Profile[{0}]  PlayMode[{1}]  ServerName[{2}]  Map[{3}]", LocalProfileId, PlayMode.ToString(), ServerName, Map);
    }
}