// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public static class NetSerializer_NetMessagePlayerIdAssignment
{
    public static int GetNetBitSize(ref NetMessagePlayerIdAssignment obj)
    {
        int result = 0;
        result += NetSerializer_PlayerId.GetNetBitSize(ref obj.playerId);
        return result;
    }

    public static void NetSerialize(ref NetMessagePlayerIdAssignment obj, BitStreamWriter writer)
    {
        NetSerializer_PlayerId.NetSerialize(ref obj.playerId, writer);
    }

    public static void NetDeserialize(ref NetMessagePlayerIdAssignment obj, BitStreamReader reader)
    {
        NetSerializer_PlayerId.NetDeserialize(ref obj.playerId, reader);
    }
}
