// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_NetMessagePlayerLeft
{
    public static int GetNetBitSize(ref NetMessagePlayerLeft obj)
    {
        int result = 0;
        result += StaticNetSerializer_PlayerId.GetNetBitSize(ref obj.playerId);
        return result;
    }

    public static void NetSerialize(ref NetMessagePlayerLeft obj, BitStreamWriter writer)
    {
        StaticNetSerializer_PlayerId.NetSerialize(ref obj.playerId, writer);
    }

    public static void NetDeserialize(ref NetMessagePlayerLeft obj, BitStreamReader reader)
    {
        StaticNetSerializer_PlayerId.NetDeserialize(ref obj.playerId, reader);
    }
}
