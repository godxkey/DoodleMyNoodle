// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public static class NetSerializer_NetMessagePlayerJoined
{
    public static int GetNetBitSize(ref NetMessagePlayerJoined obj)
    {
        int result = 0;
        result += NetSerializer_PlayerInfo.GetNetBitSize(ref obj.playerInfo);
        return result;
    }

    public static void NetSerialize(ref NetMessagePlayerJoined obj, BitStreamWriter writer)
    {
        NetSerializer_PlayerInfo.NetSerialize(ref obj.playerInfo, writer);
    }

    public static void NetDeserialize(ref NetMessagePlayerJoined obj, BitStreamReader reader)
    {
        NetSerializer_PlayerInfo.NetDeserialize(ref obj.playerInfo, reader);
    }
}
