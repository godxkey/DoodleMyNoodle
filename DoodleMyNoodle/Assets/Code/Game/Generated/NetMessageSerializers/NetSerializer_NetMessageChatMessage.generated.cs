// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;

public static class NetSerializer_NetMessageChatMessage
{
    public static int GetNetBitSize(ref NetMessageChatMessage obj)
    {
        int result = 0;
        result += NetSerializer_PlayerId.GetNetBitSize(ref obj.playerId);
        result += NetSerializer_String.GetNetBitSize(ref obj.message);
        return result;
    }

    public static void NetSerialize(ref NetMessageChatMessage obj, BitStreamWriter writer)
    {
        NetSerializer_PlayerId.NetSerialize(ref obj.playerId, writer);
        NetSerializer_String.NetSerialize(ref obj.message, writer);
    }

    public static void NetDeserialize(ref NetMessageChatMessage obj, BitStreamReader reader)
    {
        NetSerializer_PlayerId.NetDeserialize(ref obj.playerId, reader);
        NetSerializer_String.NetDeserialize(ref obj.message, reader);
    }
}
