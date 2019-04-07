// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public static class NetSerializer_NetMessagePlayerRepertoireSync
{
    public static int GetNetBitSize(ref NetMessagePlayerRepertoireSync obj)
    {
        int result = 0;
        result += ArrayNetSerializer_PlayerInfo.GetNetBitSize(ref obj.players);
        return result;
    }

    public static void NetSerialize(ref NetMessagePlayerRepertoireSync obj, BitStreamWriter writer)
    {
        ArrayNetSerializer_PlayerInfo.NetSerialize(ref obj.players, writer);
    }

    public static void NetDeserialize(ref NetMessagePlayerRepertoireSync obj, BitStreamReader reader)
    {
        ArrayNetSerializer_PlayerInfo.NetDeserialize(ref obj.players, reader);
    }
}
