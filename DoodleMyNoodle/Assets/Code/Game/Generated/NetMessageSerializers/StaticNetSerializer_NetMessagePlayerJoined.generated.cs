// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_NetMessagePlayerJoined
{
    public static int GetNetBitSize(ref NetMessagePlayerJoined obj)
    {
        int result = 0;
        result += StaticNetSerializer_PlayerInfo.GetNetBitSize_Class(obj.playerInfo);
        return result;
    }

    public static void NetSerialize(ref NetMessagePlayerJoined obj, BitStreamWriter writer)
    {
        StaticNetSerializer_PlayerInfo.NetSerialize_Class(obj.playerInfo, writer);
    }

    public static void NetDeserialize(ref NetMessagePlayerJoined obj, BitStreamReader reader)
    {
        obj.playerInfo = StaticNetSerializer_PlayerInfo.NetDeserialize_Class(reader);
    }
}
