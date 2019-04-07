// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public static class NetSerializer_NetMessagePlayerLeft
{
    public static int GetNetBitSize(ref NetMessagePlayerLeft obj)
    {
        int result = 0;
        result += NetSerializer_PlayerId.GetNetBitSize(ref obj.playerId);
        return result;
    }

    public static void NetSerialize(ref NetMessagePlayerLeft obj, BitStreamWriter writer)
    {
        NetSerializer_PlayerId.NetSerialize(ref obj.playerId, writer);
    }

    public static void NetDeserialize(ref NetMessagePlayerLeft obj, BitStreamReader reader)
    {
        NetSerializer_PlayerId.NetDeserialize(ref obj.playerId, reader);
    }
}
