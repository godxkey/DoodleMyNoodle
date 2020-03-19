// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using System;
using System.Collections.Generic;
public static class StaticNetSerializer_NetMessageChatMessage
{
    public static int GetNetBitSize(ref NetMessageChatMessage obj)
    {
        int result = 0;
        result += StaticNetSerializer_PlayerId.GetNetBitSize(ref obj.playerId);
        result += StaticNetSerializer_System_String.GetNetBitSize(ref obj.message);
        return result;
    }

    public static void NetSerialize(ref NetMessageChatMessage obj, BitStreamWriter writer)
    {
        StaticNetSerializer_PlayerId.NetSerialize(ref obj.playerId, writer);
        StaticNetSerializer_System_String.NetSerialize(ref obj.message, writer);
    }

    public static void NetDeserialize(ref NetMessageChatMessage obj, BitStreamReader reader)
    {
        StaticNetSerializer_PlayerId.NetDeserialize(ref obj.playerId, reader);
        StaticNetSerializer_System_String.NetDeserialize(ref obj.message, reader);
    }
}
